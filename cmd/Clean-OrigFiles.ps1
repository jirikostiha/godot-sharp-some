<#
.SYNOPSIS
Removes Git merge leftovers.

.DESCRIPTION
Searches for .orig files recursively under the given root and removes them.
Git leaves these behind after a conflicted merge and they are never wanted.

.PARAMETER Root
The directory to search. Defaults to the repository root, independent of the
current working directory.

.EXAMPLE
.\Clean-OrigFiles.ps1
Removes all .orig files in the repository.

.EXAMPLE
.\Clean-OrigFiles.ps1 -WhatIf
Lists the files that would be removed without touching them.
#>
<#---
name: Clean-OrigFiles
kind: cmd
description: Removes the .orig files Git leaves behind after a conflicted merge. Supports -WhatIf and -Root.
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Root = (Join-Path $PSScriptRoot "..")
)

. (Join-Path $PSScriptRoot "Common.ps1")

if ([string]::IsNullOrWhiteSpace($Root)) { $Root = (Join-Path $PSScriptRoot "..") }

Get-ChildItem -LiteralPath $Root -Recurse -File -Filter *.orig -ErrorAction SilentlyContinue |
    ForEach-Object {
        if ($PSCmdlet.ShouldProcess($_.FullName, "Remove file")) {
            Write-Host "Removing file: $($_.FullName)" -ForegroundColor Yellow
            Remove-Item -LiteralPath $_.FullName -Force
        }
    }
