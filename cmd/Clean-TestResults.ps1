<#
.SYNOPSIS
Removes TestResults folders.

.DESCRIPTION
Searches for TestResults directories recursively under the given root and
removes them with their contents. Test runs accumulate these indefinitely.

.PARAMETER Root
The directory to search. Defaults to the repository root, independent of the
current working directory.

.EXAMPLE
.\Clean-TestResults.ps1
Removes all TestResults folders in the repository.

.EXAMPLE
.\Clean-TestResults.ps1 -WhatIf
Lists the folders that would be removed without touching them.
#>
<#---
name: Clean-TestResults
kind: cmd
description: Removes the TestResults folders that accumulate from repeated test runs. Supports -WhatIf.
profiles: [dotnet]
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Root = (Join-Path $PSScriptRoot "..")
)

. (Join-Path $PSScriptRoot "Common.ps1")

if ([string]::IsNullOrWhiteSpace($Root)) { $Root = (Join-Path $PSScriptRoot "..") }

Get-ChildItem -LiteralPath $Root -Recurse -Directory -Filter 'TestResults' -ErrorAction SilentlyContinue |
    ForEach-Object {
        if ($PSCmdlet.ShouldProcess($_.FullName, "Remove directory")) {
            Write-Host "Removing directory: $($_.FullName)" -ForegroundColor Yellow
            Remove-Item -LiteralPath $_.FullName -Recurse -Force
        }
    }
