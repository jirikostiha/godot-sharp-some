<#
.SYNOPSIS
Commits and pushes the current product version.

.DESCRIPTION
Stages the file carrying the product version, commits it with a conventional
message and pushes. The file is discovered automatically; see Get-VersionFile
in Common.ps1.

.PARAMETER VersionFile
An explicit path to the version file, overriding discovery.

.PARAMETER NoPush
Commits without pushing, leaving the push to the caller.

.EXAMPLE
.\Commit-Version.ps1
Commits and pushes the current version.

.EXAMPLE
.\Commit-Version.ps1 -NoPush
Commits the version but leaves the branch unpushed.
#>
<#---
name: Commit-Version
kind: cmd
description: Commits and pushes the file carrying the product version. Use after bumping a version.
profiles: [dotnet]
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $VersionFile = (Join-Path $PSScriptRoot ".." "product_version.props"),
    [switch] $NoPush
)

. (Join-Path $PSScriptRoot "Common.ps1")

$current = Get-ProductVersion -Path $VersionFile

if ($PSCmdlet.ShouldProcess($current.Path, "Commit version $($current.Display)")) {
    Write-Host ("Committing version {0} using file: {1}" -f $current.Display, (Get-Hyperlink -Path $current.Path)) -ForegroundColor Cyan

    git add -- $current.Path
    if ($LASTEXITCODE -ne 0) { throw ("git add failed (exit code: {0})." -f $LASTEXITCODE) }

    git commit -m ("product: bump to version {0}" -f $current.Display)
    if ($LASTEXITCODE -ne 0) { throw ("git commit failed (exit code: {0})." -f $LASTEXITCODE) }

    if (-not $NoPush) {
        git push
        if ($LASTEXITCODE -ne 0) { throw ("git push failed (exit code: {0})." -f $LASTEXITCODE) }
    }
}
