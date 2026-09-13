<#
.SYNOPSIS
    Discovers C# test projects, scans initial test counts, compiles and runs them in parallel,
    logging build starts, completions, and test execution durations.
.PARAMETER RootPath
    Root directory to scan for test projects (Default: current directory).
.PARAMETER Configuration
    Build configuration used for the per-project build and test (Default: Debug).
.PARAMETER MaxDegreeOfParallelism
    Maximum number of concurrent test runs (Default: logical CPU core count).
.PARAMETER TimeoutSec
    Maximum time in seconds allowed for each project's build phase and each test phase
    before the underlying process is forcibly terminated and the project is reported as
    TimedOut (Default: 0 = no timeout / wait indefinitely).
.NOTES
    The per-project test counts shown during discovery are approximate. They are derived
    from source-level test attributes and do NOT expand data-driven cases
    (e.g. a single [Theory] with multiple [InlineData] rows counts as one).
    Commented-out attributes are ignored.
#>
<#---
name: Test-Csprojs
kind: cmd
description: Discovers C# test projects and runs them in parallel with per-project build and test timing. The default test runner; needs no solution file. Use Test-Sln for solution-level dotnet test.
profiles: [dotnet]
version: 2.2.0
---#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$RootPath = (Join-Path $PSScriptRoot ".."),

    [Parameter()]
    [string]$Configuration = "Debug",

    [Parameter()]
    [int]$MaxDegreeOfParallelism = [Environment]::ProcessorCount,

    [Parameter()]
    [int]$TimeoutSec = 0
)

# Shared helpers (verbose detection, standardized exit codes). Dot-sourced before
# Set-StrictMode so Common.ps1's own top-level runs outside Latest, like the other
# cmd scripts; the helpers called from here are strict-safe.
. (Join-Path $PSScriptRoot "Common.ps1")

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ANSI Color Codes
$cReset   = "`e[0m"
$cBold    = "`e[1m"
$cDim     = "`e[2m"
$cRed     = "`e[91m"
$cGreen   = "`e[92m"
$cYellow  = "`e[93m"
$cCyan    = "`e[96m"
$cWhite   = "`e[97m"
$cMagenta = "`e[95m"
$cBlue    = "`e[94m"

Write-Host "`n${cBold}${cCyan}=== [ C# Test Runner & Parallel Orchestrator ] ===${cReset}"
Write-Host "${cDim}Scanning for test projects in: $RootPath${cReset}"

# 1. Discover test projects
$projectFiles = Get-ChildItem -Path $RootPath -Filter *.csproj -Recurse -File | Where-Object {
    $_.FullName -notmatch '[\\/](obj|bin|\.git)[\\/]'
}

# A project is considered a test project when it references any known test SDK,
# framework or adapter, either via <PackageReference> or <ProjectReference>.
$testRefPattern = 'Microsoft\.NET\.Test\.Sdk|Microsoft\.Testing\.Platform|xunit|NUnit|MSTest\.TestFramework|MSTest\.TestAdapter'

$testProjects = @(foreach ($proj in $projectFiles) {
    try {
        [xml]$xml = Get-Content $proj.FullName -Raw
    }
    catch {
        Write-Host "${cYellow}Warning: skipping unreadable/invalid project '$($proj.FullName)': $($_.Exception.Message)${cReset}"
        continue
    }

    $refValues = @(
        ($xml.SelectNodes('//PackageReference/@Include') | ForEach-Object { $_.Value })
        ($xml.SelectNodes('//ProjectReference/@Include') | ForEach-Object { $_.Value })
    )

    $isTest = @($refValues | Where-Object { $_ -match $testRefPattern }).Count -gt 0
    if ($isTest) {
        $proj
    }
})

$totalProjects = @($testProjects).Count
if ($totalProjects -eq 0) {
    Write-Host "${cYellow}No test projects found.${cReset}`n"
    return
}

# --- LIST DISCOVERED TEST PROJECTS WITH ESTIMATED TEST COUNTS ---
Write-Host "`n${cBold}--- DISCOVERED TEST PROJECTS ($totalProjects) ---${cReset}"
$resolvedRoot = (Resolve-Path $RootPath).Path.TrimEnd('\', '/')
for ($i = 0; $i -lt $totalProjects; $i++) {
    $proj = $testProjects[$i]
    $projDir = $proj.DirectoryName
    $relPath = $proj.FullName.Substring($resolvedRoot.Length).TrimStart('\', '/')
    
    $csFiles = Get-ChildItem -Path $projDir -Filter *.cs -Recurse -File | Where-Object {
        $_.FullName -notmatch '[\\/](obj|bin)[\\/]'
    }
    $approxTests = 0
    foreach ($cs in $csFiles) {
        $content = Get-Content -Path $cs.FullName -Raw
        if ([string]::IsNullOrEmpty($content)) { continue }
        # Strip block comments (/* ... */) and line comments (// ...) so that
        # commented-out test attributes are not counted.
        $content = [regex]::Replace($content, '/\*[\s\S]*?\*/', '')
        $content = [regex]::Replace($content, '(?m)//.*$', '')
        $testMatches = [regex]::Matches($content, '\[\s*(Fact|Theory|Test|TestMethod)\b')
        $approxTests += $testMatches.Count
    }

    $indexStr = '[{0,2}]' -f ($i + 1)
    $testCountBadge = "${cCyan}~$approxTests tests${cReset}"
    Write-Host "${cDim}$indexStr${cReset} ${cWhite}$($proj.BaseName)${cReset} ${cDim}($relPath)${cReset} - $testCountBadge"
}

$timeoutLabel = if ($TimeoutSec -gt 0) { "${cDim}, Timeout: ${TimeoutSec}s/phase" } else { "${cDim}, Timeout: none" }
Write-Host "`n${cWhite}Total test projects: ${cBold}$totalProjects${cReset} ${cDim}(Parallelism: $MaxDegreeOfParallelism threads$timeoutLabel)${cReset}`n"
Write-Host "${cBold}--- EXECUTION PROGRESS ---${cReset}"

$tempResultsDir = Join-Path ([System.IO.Path]::GetTempPath()) ("DotnetTests_" + [Guid]::NewGuid().ToString("N"))
$null = New-Item -ItemType Directory -Path $tempResultsDir -Force

# Verbose detection via the shared helper, so this matches Test-Sln and the rest.
$isVerbose = Test-VerboseRequested

# Thread-safe shared state
$syncState = [hashtable]::Synchronized(@{
    StartedBuilds  = 0
    BuiltCount     = 0
    CompletedCount = 0
    TotalCount     = $totalProjects
    IsCancelled    = $false
    RunningPids    = [System.Collections.Generic.List[int]]::new()
})

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

try {
    # 2. Parallel test project execution
    $results = @($testProjects | ForEach-Object -Parallel {
        $proj           = $_
        $tempDir        = $using:tempResultsDir
        $state          = $using:syncState
        $configuration  = $using:Configuration
        $verbose        = $using:isVerbose
        $totalProjCount = $using:totalProjects
        $timeoutSec     = $using:TimeoutSec

        $cReset     = "`e[0m"
        $cRed       = "`e[91m"
        $cGreen     = "`e[92m"
        $cDim       = "`e[2m"
        $cWhite     = "`e[97m"
        $cYellow    = "`e[93m"
        $cMagenta   = "`e[95m"
        $cBlue      = "`e[94m"

        # Runs a dotnet process, streaming stdout/stderr asynchronously to avoid
        # deadlocks, registering/deregistering its PID with the shared state, and
        # enforcing an optional per-phase timeout (in seconds; 0 = wait forever).
        # Returns a hashtable: ExitCode, StdOut, StdErr, TimedOut, Cancelled.
        function Invoke-ProcessWithTimeout {
            param($Arguments, $SharedState, $TimeoutSeconds)

            $psi = New-Object System.Diagnostics.ProcessStartInfo
            $psi.FileName               = "dotnet"
            $psi.Arguments              = $Arguments
            $psi.RedirectStandardOutput = $true
            $psi.RedirectStandardError  = $true
            $psi.UseShellExecute        = $false
            $psi.CreateNoWindow         = $true

            $proc = [System.Diagnostics.Process]::Start($psi)

            [System.Threading.Monitor]::Enter($SharedState.SyncRoot)
            try {
                if ($SharedState.IsCancelled) {
                    if (-not $proc.HasExited) { $proc.Kill($true) }
                    return @{ ExitCode = -1; StdOut = ""; StdErr = ""; TimedOut = $false; Cancelled = $true }
                }
                $SharedState.RunningPids.Add($proc.Id)
            }
            finally {
                [System.Threading.Monitor]::Exit($SharedState.SyncRoot)
            }

            $outTask = $proc.StandardOutput.ReadToEndAsync()
            $errTask = $proc.StandardError.ReadToEndAsync()

            $timedOut = $false
            if ($TimeoutSeconds -gt 0) {
                if (-not $proc.WaitForExit($TimeoutSeconds * 1000)) {
                    $timedOut = $true
                    try { if (-not $proc.HasExited) { $proc.Kill($true) } } catch { }
                    try { $proc.WaitForExit() } catch { }
                }
            }
            else {
                $proc.WaitForExit()
            }

            $stdOut = ""
            $stdErr = ""
            try { $stdOut = $outTask.GetAwaiter().GetResult() } catch { }
            try { $stdErr = $errTask.GetAwaiter().GetResult() } catch { }

            [System.Threading.Monitor]::Enter($SharedState.SyncRoot)
            try {
                $SharedState.RunningPids.Remove($proc.Id) | Out-Null
            }
            finally {
                [System.Threading.Monitor]::Exit($SharedState.SyncRoot)
            }

            $exitCode = if ($timedOut) { -1 } else { $proc.ExitCode }
            return @{ ExitCode = $exitCode; StdOut = $stdOut; StdErr = $stdErr; TimedOut = $timedOut; Cancelled = $false }
        }

        if ($state.IsCancelled) { return }

        $threadId    = [System.Threading.Thread]::CurrentThread.ManagedThreadId
        $trxName     = "$($proj.BaseName)_$([Guid]::NewGuid().ToString('N')).trx"
        $trxPath     = Join-Path $tempDir $trxName
        $threadBadge = "${cMagenta}[T#$threadId]${cReset}"

        # --- NOTIFY: BUILD STARTED ---
        [System.Threading.Monitor]::Enter($state.SyncRoot)
        try {
            $state.StartedBuilds++
            $currentStarted = $state.StartedBuilds
        }
        finally {
            [System.Threading.Monitor]::Exit($state.SyncRoot)
        }

        $buildStartHeader = '[{0,2}/{1,2}]' -f $currentStarted, $totalProjCount
        [Console]::WriteLine("$cDim$buildStartHeader$cReset ${cYellow}[BUILDING]${cReset} $threadBadge ${cWhite}$($proj.BaseName)${cReset} ${cDim}(compiling...)${cReset}")

        $overallTimer = [System.Diagnostics.Stopwatch]::StartNew()

        # ----------------------------------------------------
        # STEP 1: EXPLICIT BUILD STEP
        # ----------------------------------------------------
        $buildTimer = [System.Diagnostics.Stopwatch]::StartNew()
        $buildArgs  = "build `"$($proj.FullName)`" -c $configuration --nologo -v quiet -clp:NoSummary"
        $buildRun   = Invoke-ProcessWithTimeout -Arguments $buildArgs -SharedState $state -TimeoutSeconds $timeoutSec
        $buildTimer.Stop()

        if ($buildRun.Cancelled -or $state.IsCancelled) { return }

        $buildStdOut = $buildRun.StdOut
        $buildStdErr = $buildRun.StdErr

        # Handle build timeout
        if ($buildRun.TimedOut) {
            $overallTimer.Stop()

            [System.Threading.Monitor]::Enter($state.SyncRoot)
            try {
                $state.CompletedCount++
                $current = $state.CompletedCount
            }
            finally {
                [System.Threading.Monitor]::Exit($state.SyncRoot)
            }

            $progressHeader = '[{0,2}/{1,2}]' -f $current, $totalProjCount
            $elapsedText = "{0:N2}s" -f $buildTimer.Elapsed.TotalSeconds
            [Console]::WriteLine("$cDim$progressHeader$cReset ${cYellow}${cBold}⏱ TIMEOUT ${cReset} $threadBadge ${cWhite}$($proj.BaseName)${cReset} ${cDim}[build exceeded ${timeoutSec}s]${cReset}")

            return [PSCustomObject]@{
                ProjectName      = $proj.BaseName
                Status           = "TimedOut"
                Duration         = $overallTimer.Elapsed
                Tests            = @()
                Total            = 0
                Passed           = 0
                Failed           = 0
                Skipped          = 0
                ThreadId         = $threadId
                BuildErrorOutput = "Build phase exceeded the ${timeoutSec}s timeout and was terminated."
            }
        }

        # Check for build failure
        if ($buildRun.ExitCode -ne 0) {
            $overallTimer.Stop()
            
            [System.Threading.Monitor]::Enter($state.SyncRoot)
            try {
                $state.CompletedCount++
                $current = $state.CompletedCount
            }
            finally {
                [System.Threading.Monitor]::Exit($state.SyncRoot)
            }

            $rawErrors = ($buildStdOut + "`n" + $buildStdErr).Trim()
            $buildErrorOutput = ($rawErrors -split "`r?`n" | Where-Object { $_ -match 'error\s+[A-Z0-9]+:' -or $_ -match 'FAILED:' -or $_ -match 'MSB' }) -join "`n"
            if (-not $buildErrorOutput) { $buildErrorOutput = $rawErrors }

            $progressHeader = '[{0,2}/{1,2}]' -f $current, $totalProjCount
            $elapsedText = "{0:N2}s" -f $buildTimer.Elapsed.TotalSeconds
            [Console]::WriteLine("$cDim$progressHeader$cReset ${cRed}${cBold}✖ BUILD ERR${cReset} $threadBadge ${cWhite}$($proj.BaseName)${cReset} ${cDim}[build: $elapsedText]${cReset}")

            return [PSCustomObject]@{
                ProjectName      = $proj.BaseName
                Status           = "BuildError"
                Duration         = $overallTimer.Elapsed
                Tests            = @()
                Total            = 0
                Passed           = 0
                Failed           = 0
                Skipped          = 0
                ThreadId         = $threadId
                BuildErrorOutput = $buildErrorOutput
            }
        }

        # --- NOTIFY: BUILD SUCCEEDED ---
        [System.Threading.Monitor]::Enter($state.SyncRoot)
        try {
            $state.BuiltCount++
            $currentBuilt = $state.BuiltCount
        }
        finally {
            [System.Threading.Monitor]::Exit($state.SyncRoot)
        }

        $buildProgressHeader = '[{0,2}/{1,2}]' -f $currentBuilt, $totalProjCount
        $buildElapsedText    = "{0:N2}s" -f $buildTimer.Elapsed.TotalSeconds
        [Console]::WriteLine("$cDim$buildProgressHeader$cReset ${cBlue}[BUILD OK]${cReset}  $threadBadge ${cWhite}$($proj.BaseName)${cReset} ${cDim}(built in $buildElapsedText -> running tests...)${cReset}")

        # ----------------------------------------------------
        # STEP 2: TEST EXECUTION (--no-build)
        # ----------------------------------------------------
        $testTimer = [System.Diagnostics.Stopwatch]::StartNew()
        $testArgsBase = "test `"$($proj.FullName)`" -c $configuration --nologo -v quiet --logger `"trx;LogFileName=$trxName`" --results-directory `"$tempDir`""
        $testRun = Invoke-ProcessWithTimeout -Arguments "$testArgsBase --no-build" -SharedState $state -TimeoutSeconds $timeoutSec

        if ($testRun.Cancelled -or $state.IsCancelled) { return }

        # Fallback: multi-targeted or edge-case projects can fail '--no-build' with
        # no produced TRX. Retry once WITH a build so results are not silently lost.
        if (-not $testRun.TimedOut -and $testRun.ExitCode -ne 0 -and -not (Test-Path $trxPath)) {
            [Console]::WriteLine("$cDim[  retry ]$cReset ${cYellow}[RE-RUN]${cReset}   $threadBadge ${cWhite}$($proj.BaseName)${cReset} ${cDim}(--no-build failed, retrying with build...)${cReset}")
            $testRun = Invoke-ProcessWithTimeout -Arguments $testArgsBase -SharedState $state -TimeoutSeconds $timeoutSec
            if ($testRun.Cancelled -or $state.IsCancelled) { return }
        }

        $testTimer.Stop()
        $overallTimer.Stop()

        # Handle test timeout
        if ($testRun.TimedOut) {
            [System.Threading.Monitor]::Enter($state.SyncRoot)
            try {
                $state.CompletedCount++
                $current = $state.CompletedCount
            }
            finally {
                [System.Threading.Monitor]::Exit($state.SyncRoot)
            }

            $progressHeader = '[{0,2}/{1,2}]' -f $current, $totalProjCount
            $totalSecsText  = "{0:N2}s" -f $overallTimer.Elapsed.TotalSeconds
            [Console]::WriteLine("$cDim$progressHeader$cReset ${cYellow}${cBold}⏱ TIMEOUT ${cReset} $threadBadge ${cWhite}$($proj.BaseName)${cReset} ${cDim}[test exceeded ${timeoutSec}s | total: $totalSecsText]${cReset}")

            return [PSCustomObject]@{
                ProjectName      = $proj.BaseName
                Status           = "TimedOut"
                Duration         = $overallTimer.Elapsed
                Tests            = @()
                Total            = 0
                Passed           = 0
                Failed           = 0
                Skipped          = 0
                ThreadId         = $threadId
                BuildErrorOutput = "Test phase exceeded the ${timeoutSec}s timeout and was terminated."
            }
        }

        $tests = @()
        $trxXml = $null

        if (Test-Path $trxPath) {
            try {
                [xml]$trxXml = Get-Content -Path $trxPath -Raw
            }
            catch {
                $trxXml = $null
            }
        }
        if ($trxXml) {
            $ns = @{ d = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010" }

            $unitTestResults = Select-Xml -Xml $trxXml -XPath "//d:UnitTestResult" -Namespace $ns
            foreach ($res in $unitTestResults) {
                $node = $res.Node
                $testOutcome = $node.outcome
                $rawDuration = $node.duration
                $testName    = $node.testName

                $formattedDuration = ""
                if ($rawDuration) {
                    try {
                        $ts = [TimeSpan]::Parse($rawDuration)
                        if ($ts.TotalSeconds -ge 1.0) {
                            $formattedDuration = "{0:N2}s" -f $ts.TotalSeconds
                        } elseif ($ts.TotalMilliseconds -gt 0) {
                            $formattedDuration = "{0:N0}ms" -f $ts.TotalMilliseconds
                        } else {
                            $formattedDuration = "<1ms"
                        }
                    } catch {
                        $formattedDuration = ""
                    }
                }

                $errMsg   = ""
                $errStack = ""
                $nsMgr    = New-Object System.Xml.XmlNamespaceManager($trxXml.NameTable)
                $nsMgr.AddNamespace("d", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")

                $errNode = $node.SelectSingleNode("d:Output/d:ErrorInfo", $nsMgr)
                if ($errNode) {
                    $mNode = $errNode.SelectSingleNode("d:Message", $nsMgr)
                    $sNode = $errNode.SelectSingleNode("d:StackTrace", $nsMgr)
                    if ($mNode) { $errMsg = $mNode.InnerText }
                    if ($sNode) { $errStack = $sNode.InnerText }
                }

                $tests += [PSCustomObject]@{
                    Name         = $testName
                    Outcome      = $testOutcome
                    Duration     = $formattedDuration
                    ErrorMessage = $errMsg
                    StackTrace   = $errStack
                    ThreadId     = $threadId
                }
            }
        }

        $totalTestCount   = @($tests).Count
        $passedTestCount  = @($tests | Where-Object { $_.Outcome -eq 'Passed' }).Count
        $failedTestCount  = @($tests | Where-Object { $_.Outcome -eq 'Failed' }).Count
        $skippedTestCount = @($tests | Where-Object { $_.Outcome -in @('NotExecuted', 'Ignored', 'Skipped') }).Count

        $status = if ($testRun.ExitCode -ne 0 -or $failedTestCount -gt 0) {
            "Failed"
        } elseif ($totalTestCount -eq 0) {
            "NoTests"
        } else {
            "Passed"
        }

        # Final Progress update
        [System.Threading.Monitor]::Enter($state.SyncRoot)
        try {
            $state.CompletedCount++
            $current = $state.CompletedCount
        }
        finally {
            [System.Threading.Monitor]::Exit($state.SyncRoot)
        }

        $badge = switch ($status) {
            "Passed"     { "${cGreen}✓ PASSED  ${cReset}" }
            "Failed"     { "${cRed}✖ FAILED  ${cReset}" }
            "NoTests"    { "${cYellow}○ NO TESTS${cReset}" }
            default      { "${cYellow}○ NO TESTS${cReset}" }
        }

        $progressHeader = '[{0,2}/{1,2}]' -f $current, $totalProjCount
        $totalSecsText  = "{0:N2}s" -f $overallTimer.Elapsed.TotalSeconds
        $testSecsText   = "{0:N2}s" -f $testTimer.Elapsed.TotalSeconds
        $countBadge     = "${cDim}($totalTestCount tests)${cReset}"

        [Console]::WriteLine("$cDim$progressHeader$cReset $badge $threadBadge ${cWhite}$($proj.BaseName)${cReset} $countBadge ${cDim}[test: $testSecsText | total: $totalSecsText]${cReset}")

        # Individual unit tests on -Verbose
        if ($verbose -and $totalTestCount -gt 0) {
            foreach ($t in $tests) {
                $tOutcomeBadge = switch ($t.Outcome) {
                    'Passed'  { "${cGreen}  ✓${cReset}" }
                    'Failed'  { "${cRed}  ✖${cReset}" }
                    default   { "${cYellow}  ○${cReset}" }
                }
                $durDisplay = if ($t.Duration) { " ${cDim}($($t.Duration))${cReset}" } else { "" }
                [Console]::WriteLine("   $tOutcomeBadge ${cDim}$threadBadge${cReset} $($t.Name)$durDisplay")
            }
        }

        [PSCustomObject]@{
            ProjectName      = $proj.BaseName
            Status           = $status
            Duration         = $overallTimer.Elapsed
            Tests            = $tests
            Total            = $totalTestCount
            Passed           = $passedTestCount
            Failed           = $failedTestCount
            Skipped          = $skippedTestCount
            ThreadId         = $threadId
            BuildErrorOutput = ""
        }
    } -ThrottleLimit $MaxDegreeOfParallelism)

    $stopwatch.Stop()

    # Filter out null results (from cancelled/skipped parallel runs) so downstream
    # aggregation is safe under Set-StrictMode.
    $results = @($results | Where-Object { $null -ne $_ })

    if ($syncState.IsCancelled) {
        Write-Host "`n${cYellow}Execution cancelled by user.${cReset}"
        return
    }

    # 3. Final Formatted Overview Table
    Write-Host "`n${cBold}--- SUITE RESULTS OVERVIEW ---${cReset}"

    $headerFmt = '{0,-38} {1,-10} {2,7} {3,7} {4,7} {5,6} {6,10}'
    Write-Host ($headerFmt -f "Project", "Status", "Total", "Passed", "Failed", "Skip", "Duration") -ForegroundColor Gray
    Write-Host ("-" * 89) -ForegroundColor DarkGray

    foreach ($r in $results) {
        if ($null -eq $r) { continue }
        
        $projDisplay = if ($r.ProjectName.Length -gt 38) { $r.ProjectName.Substring(0, 35) + "..." } else { $r.ProjectName }

        $coloredBadge = switch ($r.Status) {
            "Passed"     { "[ ${cGreen}PASS${cReset} ]" }
            "Failed"     { "[ ${cRed}FAIL${cReset} ]" }
            "BuildError" { "[${cRed}${cBold}BUILD${cReset}]" }
            "TimedOut"   { "[${cYellow}${cBold}TIME${cReset} ]" }
            "NoTests"    { "[${cYellow}EMPTY${cReset}]" }
            Default      { "[UNKNW]" }
        }

        $pName  = '{0,-38}' -f $projDisplay
        $sTotal = '{0,7}'   -f $r.Total
        
        $sPassed = '{0,7}' -f $r.Passed
        if ($r.Passed -gt 0) { $sPassed = "${cGreen}$sPassed${cReset}" }

        $sFailed = '{0,7}' -f $r.Failed
        if ($r.Failed -gt 0) { $sFailed = "${cRed}${cBold}$sFailed${cReset}" }

        $sSkip = '{0,6}' -f $r.Skipped
        if ($r.Skipped -gt 0) { $sSkip = "${cYellow}$sSkip${cReset}" }

        $sDur = '{0,10}' -f ("{0:N2}s" -f $r.Duration.TotalSeconds)

        Write-Host "$pName $coloredBadge $sTotal $sPassed $sFailed $sSkip $sDur"
    }

    Write-Host ("-" * 89) -ForegroundColor DarkGray

    # 4. Detailed Build Errors Section
    $buildFailures = @($results | Where-Object { $_.Status -eq 'BuildError' })
    if ($buildFailures.Count -gt 0) {
        Write-Host "`n${cRed}${cBold}=== BUILD / COMPILATION ERRORS ($($buildFailures.Count)) ===${cReset}"
        foreach ($bf in $buildFailures) {
            Write-Host "`n${cRed}✖ $($bf.ProjectName)${cReset} ${cMagenta}[Thread #$($bf.ThreadId)]${cReset}"
            if ($bf.BuildErrorOutput) {
                Write-Host "${cDim}$($bf.BuildErrorOutput.Trim() -replace '(?m)^', '    ')${cReset}"
            } else {
                Write-Host "${cDim}    Build failed without standard compiler diagnostics.${cReset}"
            }
        }
    }

    # 4b. Detailed Timeout Section
    $timedOutProjects = @($results | Where-Object { $_.Status -eq 'TimedOut' })
    if ($timedOutProjects.Count -gt 0) {
        Write-Host "`n${cYellow}${cBold}=== TIMED-OUT PROJECTS ($($timedOutProjects.Count)) ===${cReset}"
        foreach ($to in $timedOutProjects) {
            Write-Host "`n${cYellow}⏱ $($to.ProjectName)${cReset} ${cMagenta}[Thread #$($to.ThreadId)]${cReset}"
            if ($to.BuildErrorOutput) {
                Write-Host "${cDim}    $($to.BuildErrorOutput.Trim())${cReset}"
            }
        }
    }

    # 5. Detailed Test Failures Section
    $allFailedTests = @(foreach ($res in $results) {
        if ($res.Tests) {
            $res.Tests | Where-Object { $_.Outcome -eq 'Failed' }
        }
    })

    if ($allFailedTests.Count -gt 0) {
        Write-Host "`n${cRed}${cBold}=== FAILED TESTS BREAKDOWN ($($allFailedTests.Count)) ===${cReset}"
        foreach ($failed in $allFailedTests) {
            Write-Host "`n${cRed}✖ $($failed.Name)${cReset} ${cMagenta}[Thread #$($failed.ThreadId)]${cReset}"
            if ($failed.ErrorMessage) {
                Write-Host "${cWhite}  Error:${cReset} $($failed.ErrorMessage.Trim())"
            }
            if ($failed.StackTrace) {
                Write-Host "${cDim}$($failed.StackTrace.Trim() -replace '(?m)^', '    ')${cReset}"
            }
        }
    }

    # 6. Aligned Execution Summary
    $totalPassed    = (@($results | ForEach-Object { $_.Passed }) | Measure-Object -Sum).Sum
    $totalFailed    = (@($results | ForEach-Object { $_.Failed }) | Measure-Object -Sum).Sum
    $totalSkipped   = (@($results | ForEach-Object { $_.Skipped }) | Measure-Object -Sum).Sum
    $totalTests     = (@($results | ForEach-Object { $_.Total }) | Measure-Object -Sum).Sum
    $totalBuildErrs = $buildFailures.Count
    $totalTimedOut  = $timedOutProjects.Count

    Write-Host "`n${cBold}--- EXECUTION SUMMARY ---${cReset}"
    
    $lblFmt = '{0,-18}: {1,6}'
    
    Write-Host ($lblFmt -f "  Projects Total", $totalProjects) -ForegroundColor White
    
    $bErrStr = if ($totalBuildErrs -gt 0) { "${cRed}${cBold}$('{0,6}' -f $totalBuildErrs)${cReset}" } else { "     0" }
    Write-Host ("  {0,-16}: $bErrStr" -f "Build Errors")

    $toStr = if ($totalTimedOut -gt 0) { "${cYellow}${cBold}$('{0,6}' -f $totalTimedOut)${cReset}" } else { "     0" }
    Write-Host ("  {0,-16}: $toStr" -f "Timed Out")

    Write-Host ($lblFmt -f "  Tests Total", $totalTests) -ForegroundColor White
    
    $pValStr = if ($totalPassed -gt 0) { "${cGreen}$('{0,6}' -f $totalPassed)${cReset}" } else { "     0" }
    Write-Host ("  {0,-16}: $pValStr" -f "Tests Passed")

    $fValStr = if ($totalFailed -gt 0) { "${cRed}${cBold}$('{0,6}' -f $totalFailed)${cReset}" } else { "     0" }
    Write-Host ("  {0,-16}: $fValStr" -f "Tests Failed")

    $sValStr = if ($totalSkipped -gt 0) { "${cYellow}$('{0,6}' -f $totalSkipped)${cReset}" } else { "     0" }
    Write-Host ("  {0,-16}: $sValStr" -f "Tests Skipped")

    $wallTimeStr = "{0:N2}s" -f $stopwatch.Elapsed.TotalSeconds
    Write-Host ("  {0,-16}: ${cCyan}$('{0,6}' -f $wallTimeStr)${cReset} ${cDim}(parallel execution)${cReset}`n" -f "Total Wall Time")

    # Standardized exit codes (see Get-AiKitExitCode in Common.ps1), consistent with
    # the other cmd scripts: test failures or timeouts report Test, pure build
    # failures report Build. The finally block below still runs to clean up.
    if ($totalFailed -gt 0 -or $totalTimedOut -gt 0) {
        exit (Get-AiKitExitCode Test)
    }
    if ($totalBuildErrs -gt 0) {
        exit (Get-AiKitExitCode Build)
    }
}
finally {
    if ($syncState) {
        $syncState.IsCancelled = $true
        [System.Threading.Monitor]::Enter($syncState.SyncRoot)
        try {
            foreach ($pidToKill in $syncState.RunningPids) {
                try {
                    $p = [System.Diagnostics.Process]::GetProcessById($pidToKill)
                    if (-not $p.HasExited) { $p.Kill($true) }
                } catch { }
            }
            $syncState.RunningPids.Clear()
        }
        finally {
            [System.Threading.Monitor]::Exit($syncState.SyncRoot)
        }
    }

    if (Test-Path $tempResultsDir) {
        Remove-Item -Path $tempResultsDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}