<#
.SYNOPSIS
Commits and pushes the current product version.

.DESCRIPTION
Stages the file carrying the product version, commits it with a conventional
message and pushes. The file is discovered automatically; see Get-VersionFile
in Common.ps1.

.PARAMETER Prefix
The version the caller has just set, named in the commit message. Only -WhatIf, which
leaves the file alone, reports it without checking; a real run refuses a version the file
does not carry, so the message never misstates what is committed. It is normalised to
three parts, so 1.6 names 1.6.0.

.PARAMETER Suffix
The stage suffix the caller has just set, checked like -Prefix.

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
version: 2.1.1
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [version] $Prefix,
    [string] $Suffix,
    [string] $VersionFile = (Join-Path $PSScriptRoot ".." "product_version.props"),
    [switch] $NoPush
)

. (Join-Path $PSScriptRoot "Common.ps1")

$current = Get-ProductVersion -Path $VersionFile

$versionPrefix = if ($PSBoundParameters.ContainsKey('Prefix')) { ConvertTo-ThreePartVersion $Prefix } else { $current.Prefix }
$versionSuffix = if ($PSBoundParameters.ContainsKey('Suffix')) { $Suffix } else { $current.Suffix }
$display = if ($versionSuffix) { "$versionPrefix-$versionSuffix" } else { "$versionPrefix" }

if (-not $WhatIfPreference -and
    ((ConvertTo-ThreePartVersion $versionPrefix) -ne (ConvertTo-ThreePartVersion $current.Prefix) -or [string] $versionSuffix -ne [string] $current.Suffix)) {
    throw ("The version file carries {0}, not {1}; set the version before committing it." -f $current.Display, $display)
}

if ($PSCmdlet.ShouldProcess($current.Path, "Commit version $display")) {
    Write-Host ("Committing version {0} using file: {1}" -f $display, (Get-Hyperlink -Path $current.Path)) -ForegroundColor Cyan

    git add -- $current.Path
    if ($LASTEXITCODE -ne 0) { throw ("git add failed (exit code: {0})." -f $LASTEXITCODE) }

    git commit -m ("product: bump to version {0}" -f $display)
    if ($LASTEXITCODE -ne 0) { throw ("git commit failed (exit code: {0})." -f $LASTEXITCODE) }

    if (-not $NoPush) {
        git push
        if ($LASTEXITCODE -ne 0) { throw ("git push failed (exit code: {0})." -f $LASTEXITCODE) }
    }
}
