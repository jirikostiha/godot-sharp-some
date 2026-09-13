<#
.SYNOPSIS
Removes build output folders.

.DESCRIPTION
Searches for obj, bin and artf folders recursively under the given root and
removes them, so the next build starts from a clean state.

.PARAMETER Root
The directory to search. Defaults to the repository root, independent of the
current working directory.

.EXAMPLE
.\Clean-Binaries.ps1
Removes all build output folders in the repository.

.EXAMPLE
.\Clean-Binaries.ps1 -WhatIf
Lists the folders that would be removed without touching them.
#>
<#---
name: Clean-Binaries
kind: cmd
description: Removes obj, bin and artf folders so the next build starts clean. Supports -WhatIf.
profiles: [dotnet]
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Root = (Join-Path $PSScriptRoot "..")
)

. (Join-Path $PSScriptRoot "Common.ps1")

if ([string]::IsNullOrWhiteSpace($Root)) { $Root = (Join-Path $PSScriptRoot "..") }

foreach ($name in @("obj", "bin", "artf")) {
    Write-Host "Searching for '$name' folders under $Root..." -ForegroundColor Magenta

    Get-ChildItem -LiteralPath $Root -Recurse -Directory -Filter $name -ErrorAction SilentlyContinue |
        ForEach-Object {
            if ($PSCmdlet.ShouldProcess($_.FullName, "Remove folder")) {
                Write-Host "Removing folder: $($_.FullName)" -ForegroundColor Yellow
                Remove-Item -LiteralPath $_.FullName -Recurse -Force
            }
        }
}
