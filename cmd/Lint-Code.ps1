<#
.SYNOPSIS
Formats source code to match the editorconfig settings.

.DESCRIPTION
Runs dotnet format on the main solution so the code style stays consistent.
The solution is discovered automatically; see Get-SolutionPath in Common.ps1.

.PARAMETER Verify
Verifies that no formatting changes are required without applying them, and
fails when any file would have been reformatted. Use this in a release check.

.PARAMETER Solution
An explicit solution path, overriding discovery.

.EXAMPLE
.\Lint-Code.ps1
Applies formatting fixes to the solution.

.EXAMPLE
.\Lint-Code.ps1 -Verify
Reports formatting violations without changing anything.
#>
<#---
name: Lint-Code
kind: cmd
description: Formats the solution with dotnet format, or verifies formatting without changing files.
profiles: [dotnet]
version: 2.2.0
---#>
[CmdletBinding()]
param(
    [switch] $Verify,
    [string] $Solution
)

. (Join-Path $PSScriptRoot "Common.ps1")

try { $slnPath = Get-SolutionPath -Path $Solution } catch { Stop-Script -Reason Precondition -Message $_.Exception.Message }

# Quiet on success, the offending files on failure; -Verbose streams full formatter detail.
$verbosityArgs = if (Test-VerboseRequested) { @("-v", "diagnostic") } else { @("-v", "quiet") }

if ($Verify) {
    Write-Verbose ("Verifying code formatting for: {0}" -f $slnPath)
    Invoke-Tool -FilePath "dotnet" `
        -Arguments (@("format", $slnPath, "--verify-no-changes") + $verbosityArgs) `
        -FailReason Lint -FailMessage "code formatting violations found"
}
else {
    Write-Verbose ("Formatting code for: {0}" -f $slnPath)
    Invoke-Tool -FilePath "dotnet" `
        -Arguments (@("format", $slnPath) + $verbosityArgs) `
        -FailReason Lint -FailMessage "dotnet format failed"
}
