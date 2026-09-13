<#
.SYNOPSIS
Builds the repository solution.

.DESCRIPTION
Builds the main solution file with dotnet build. The solution is discovered
automatically; see Get-SolutionPath in Common.ps1.

.PARAMETER Configuration
The build configuration to use, for example Debug or Release. Defaults to Debug.

.PARAMETER Solution
An explicit solution path, overriding discovery.

.PARAMETER AdditionalArgs
Any remaining arguments are passed through to dotnet build unchanged.

.EXAMPLE
.\Build-Sln.ps1
Builds the solution in Debug configuration.

.EXAMPLE
.\Build-Sln.ps1 -Configuration Release --no-incremental
Builds in Release and passes --no-incremental through to dotnet.
#>
<#---
name: Build-Sln
kind: cmd
description: Builds the repository solution with dotnet build. Use to compile the whole repository locally.
profiles: [dotnet]
version: 2.2.0
---#>
[CmdletBinding()]
param(
    [string] $Configuration = "Debug",
    [string] $Solution,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $AdditionalArgs
)

. (Join-Path $PSScriptRoot "Common.ps1")

try { $slnPath = Get-SolutionPath -Path $Solution } catch { Stop-Script -Reason Precondition -Message $_.Exception.Message }
Write-Verbose ("Building solution: {0}" -f $slnPath)

# Quiet on success, real compiler errors on failure; -Verbose streams the full build.
$verbosityArgs = if (Test-VerboseRequested) { @() } else { @("--nologo", "-v", "quiet") }
Invoke-Tool -FilePath "dotnet" `
    -Arguments (@("build", $slnPath, "-c", $Configuration) + $verbosityArgs + $AdditionalArgs) `
    -FailReason Build -FailMessage "dotnet build failed"
