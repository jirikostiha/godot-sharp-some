<#
.SYNOPSIS
Runs the solution's tests with dotnet test.

.DESCRIPTION
Runs the unit and integration tests with dotnet test on the main solution file.
The solution is discovered automatically; see Get-SolutionPath in Common.ps1.

Works with either dotnet test runner: in Microsoft.Testing.Platform mode (selected
by global.json, see Test-MtpTestRunner in Common.ps1) the solution is passed with
--solution, otherwise positionally as VSTest expects.

This script is solution-level only. A repository without a solution file is a
precondition error here; test its projects directly with Test-Csprojs.ps1, which
is the default runner and needs no solution.

Output is asymmetric like the other cmd scripts: quiet on success, the failing
tests on failure. -Verbose streams the full test run.

.PARAMETER Configuration
The build configuration to use, for example Debug or Release. Defaults to Debug.

.PARAMETER Solution
An explicit solution path, overriding discovery. A path that cannot be found is
an error rather than a fallback.

.PARAMETER AdditionalArgs
Any remaining arguments are passed through to dotnet test unchanged, for example
--no-build to reuse an earlier Build-Sln.ps1.

.EXAMPLE
.\Test-Sln.ps1
Executes the full test suite in Debug configuration.

.EXAMPLE
.\Test-Sln.ps1 -Configuration Release --no-build
Runs the Release tests without rebuilding first.
#>
<#---
name: Test-Sln
kind: cmd
description: Runs the whole test suite with dotnet test on the solution. Use to verify a solution-based repository before committing or releasing; for a repository without a solution use Test-Csprojs.
profiles: [dotnet]
version: 2.5.0
---#>
# PositionalBinding off: -Configuration and -Solution bind only by name, so a bare
# argument such as --no-build reaches $AdditionalArgs instead of a parameter slot.
[CmdletBinding(PositionalBinding = $false)]
param(
    [string] $Configuration = "Debug",
    [string] $Solution,
    [Parameter(ValueFromRemainingArguments = $true)]
    # Defaults to empty: an unbound $null would reach dotnet as an empty argument,
    # which MTP mode forwards to the test applications, and they then run nothing.
    [string[]] $AdditionalArgs = @()
)

. (Join-Path $PSScriptRoot "Common.ps1")

# Solution-level only: dotnet test drives the solution. Without one, this is a
# precondition error rather than a fallback; Test-Csprojs.ps1 is the runner that
# needs no solution. See Resolve-SolutionPath in Common.ps1.
try { $slnPath = Resolve-SolutionPath -Path $Solution } catch { Stop-Script -Reason Precondition -Message $_.Exception.Message }

if (-not $slnPath) {
    Stop-Script -Reason Precondition -Message "No solution file found. Test the projects directly with Test-Csprojs.ps1."
}

Write-Verbose ("Testing solution: {0}" -f $slnPath)

# dotnet test picks its runner from the global.json nearest its working directory,
# so run it from the solution's folder and detect the runner from the same place.
$slnFolder = Split-Path -Parent $slnPath
$isMtp = Test-MtpTestRunner -Path $slnFolder

# MTP mode hands a positional path and unknown options such as --nologo to the test
# applications, which then run nothing; VSTest mode has no --solution.
$targetArgs = if ($isMtp) { @("--solution", $slnPath) } else { @($slnPath) }

# Quiet on success, failing tests on failure; -Verbose streams the full test run.
$verbosityArgs = if (Test-VerboseRequested) { @() } elseif ($isMtp) { @("-v", "quiet") } else { @("--nologo", "-v", "quiet") }

Push-Location -LiteralPath $slnFolder
try {
    Invoke-Tool -FilePath "dotnet" `
        -Arguments (@("test") + $targetArgs + @("-c", $Configuration) + $verbosityArgs + $AdditionalArgs) `
        -FailReason Test -FailMessage "dotnet test failed"
}
finally {
    Pop-Location
}
