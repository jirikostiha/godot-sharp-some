<#
.SYNOPSIS
Checks out a tag in a detached HEAD.

.DESCRIPTION
Fetches the tags, verifies that the requested tag exists and checks it out in a
detached HEAD, so an inspection of a released state cannot accidentally commit
onto a branch.

.PARAMETER Tag
The tag to check out, for example v1.4.0.

.PARAMETER RepoPath
The repository to act on. Defaults to the repository this script belongs to.

.PARAMETER SkipFetch
Skips fetching tags from the remotes before the checkout.

.EXAMPLE
.\Checkout-Tag.ps1 -Tag v1.4.0
Checks out tag v1.4.0.

.EXAMPLE
.\Checkout-Tag.ps1 -Tag v1.4.0 -SkipFetch
Checks out a tag already present locally, without touching the network.
#>
<#---
name: Checkout-Tag
kind: cmd
description: Checks out a released tag in a detached HEAD after verifying it exists. Use to inspect a release.
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string] $Tag,

    [string] $RepoPath = (Join-Path $PSScriptRoot ".."),

    [switch] $SkipFetch
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "Common.ps1")

if ([string]::IsNullOrWhiteSpace($RepoPath)) { $RepoPath = (Join-Path $PSScriptRoot "..") }
$resolvedRepoPath = (Resolve-Path -LiteralPath $RepoPath).Path

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "git command was not found in PATH."
}

Push-Location -LiteralPath $resolvedRepoPath
try {
    if (-not $SkipFetch -and $PSCmdlet.ShouldProcess($resolvedRepoPath, "Fetch tags from remotes")) {
        Write-Host "Fetching tags in repository '$resolvedRepoPath'." -ForegroundColor Cyan
        & git fetch --all --tags --prune
        if ($LASTEXITCODE -ne 0) { throw "git fetch failed in '$resolvedRepoPath'." }
    }

    & git rev-parse --verify --quiet "refs/tags/$Tag" | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Tag '$Tag' was not found in repository '$resolvedRepoPath'."
    }

    if ($PSCmdlet.ShouldProcess($resolvedRepoPath, "Checkout tag '$Tag'")) {
        Write-Host "Checking out tag '$Tag' (detached HEAD)." -ForegroundColor Cyan
        & git checkout --detach "tags/$Tag"
        if ($LASTEXITCODE -ne 0) { throw "git checkout failed for tag '$Tag'." }
    }
}
finally {
    Pop-Location
}
