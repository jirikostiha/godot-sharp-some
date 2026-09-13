<#
.SYNOPSIS
Runs the solution's tests with dotnet test.

.DESCRIPTION
Runs the unit and integration tests with dotnet test on the main solution file.
The solution is discovered automatically; see Get-SolutionPath in Common.ps1.

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
version: 2.4.0
---#>
[CmdletBinding()]
param(
    [string] $Configuration = "Debug",
    [string] $Solution,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $AdditionalArgs
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

# Quiet on success, failing tests on failure; -Verbose streams the full test run.
$verbosityArgs = if (Test-VerboseRequested) { @() } else { @("--nologo", "-v", "quiet") }
Invoke-Tool -FilePath "dotnet" `
    -Arguments (@("test", $slnPath, "-c", $Configuration) + $verbosityArgs + $AdditionalArgs) `
    -FailReason Test -FailMessage "dotnet test failed"
