<#
.SYNOPSIS
Bumps the product version and sets the stage suffix.

.DESCRIPTION
Increments the build or minor part of the product version and optionally sets
or clears the stage suffix. The file carrying the version is discovered
automatically and both conventions are supported — a dedicated
product_version.props, or the version held in a project or props file. See
Get-VersionFile in Common.ps1.

.PARAMETER Minor
Bumps the minor version and resets the build part to zero. Without it the build
part is incremented.

.PARAMETER NoBump
Leaves the version prefix alone. Use it to change only the suffix.

.PARAMETER Suffix
The stage suffix to set, for example dev, rc or rel.

.PARAMETER ClearSuffix
Clears the stage suffix.

.PARAMETER VersionFile
An explicit path to the version file, overriding discovery.

.EXAMPLE
.\Bump-Version.ps1
Bumps the build version, for example 1.4.1 to 1.4.2.

.EXAMPLE
.\Bump-Version.ps1 -Minor -Suffix dev
Bumps the minor version and marks it as a development build, for example 1.4.1 to 1.5.0-dev.

.EXAMPLE
.\Bump-Version.ps1 -NoBump -ClearSuffix
Promotes the current version to a release by dropping the suffix.
#>
<#---
name: Bump-Version
kind: cmd
description: Increments the product version and sets or clears the stage suffix. Use when preparing a release.
profiles: [dotnet]
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [switch] $Minor,
    [switch] $NoBump,
    [string] $Suffix,
    [switch] $ClearSuffix,
    [string] $VersionFile = (Join-Path $PSScriptRoot ".." "product_version.props")
)

. (Join-Path $PSScriptRoot "Common.ps1")

if ($ClearSuffix -and $PSBoundParameters.ContainsKey('Suffix')) {
    throw "-Suffix and -ClearSuffix contradict each other; pass only one."
}

$current = Get-ProductVersion -Path $VersionFile
$old = $current.Prefix

$new = if ($NoBump) {
    $old
}
elseif ($Minor) {
    [version] ("{0}.{1}.0" -f $old.Major, ($old.Minor + 1))
}
else {
    [version] ("{0}.{1}.{2}" -f $old.Major, $old.Minor, ($old.Build + 1))
}

$newSuffix = if ($ClearSuffix) {
    ""
}
elseif ($PSBoundParameters.ContainsKey('Suffix')) {
    $Suffix
}
else {
    $current.Suffix
}

$newDisplay = if ($newSuffix) { "$new-$newSuffix" } else { "$new" }

Write-Host ("Updating file: {0}" -f (Get-Hyperlink -Path $current.Path)) -ForegroundColor Cyan
Write-Host ("{0} -> {1}" -f $current.Display, $newDisplay) -ForegroundColor Yellow

if ($PSCmdlet.ShouldProcess($current.Path, "Set version to $newDisplay")) {
    Set-ProductVersion -Path $current.Path -Prefix $new -Suffix $newSuffix -Confirm:$false
    Write-Host "Version updated successfully." -ForegroundColor Green
}
