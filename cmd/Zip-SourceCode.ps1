<#
.SYNOPSIS
Zips the source code of the repository.

.DESCRIPTION
Creates a ZIP archive of the repository, leaving out build output, test results
and version control folders, so the archive holds source only.

.PARAMETER SourceFolder
The directory to archive. Defaults to the repository root.

.PARAMETER OutputFile
Where to write the archive. Defaults to temp/source_code.zip in the repository.

.PARAMETER Exclude
Name patterns to leave out of the archive.

.EXAMPLE
.\Zip-SourceCode.ps1
Archives the repository source into temp/source_code.zip.

.EXAMPLE
.\Zip-SourceCode.ps1 -OutputFile ..\backup.zip
Writes the archive to a chosen location.
#>
<#---
name: Zip-SourceCode
kind: cmd
description: Archives the repository source into a ZIP, excluding build output and version control folders.
version: 2.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $SourceFolder = (Join-Path $PSScriptRoot ".."),
    [string] $OutputFile = (Join-Path $PSScriptRoot ".." "temp" "source_code.zip"),
    [string[]] $Exclude = @("bin", "obj", "artf", ".git", ".vs", ".ai", "TestResults", "CoverageReport", "temp")
)

. (Join-Path $PSScriptRoot "Common.ps1")

if ([string]::IsNullOrWhiteSpace($SourceFolder)) { $SourceFolder = (Join-Path $PSScriptRoot "..") }
if ([string]::IsNullOrWhiteSpace($OutputFile)) {
    $OutputFile = Join-Path (Join-Path $PSScriptRoot "..") "temp" "source_code.zip"
}

$SourceFolder = (Get-Item -LiteralPath $SourceFolder -ErrorAction Stop).FullName

# Normalise without requiring the target to exist, so the destination folder is
# created only when the archive is actually written, and never under -WhatIf.
$OutputFile = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputFile)
$outputDir = Split-Path -Path $OutputFile -Parent
if (-not $outputDir) {
    $outputDir = (Get-Location).Path
    $OutputFile = Join-Path $outputDir (Split-Path -Path $OutputFile -Leaf)
}

if ($PSCmdlet.ShouldProcess($OutputFile, "Create ZIP archive from $SourceFolder")) {
    if (-not (Test-Path -LiteralPath $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }

    $tempDir = Join-Path ([IO.Path]::GetTempPath()) ([guid]::NewGuid().ToString())

    try {
        Write-Verbose "Staging files to $tempDir..."
        # -Force so hidden entries such as .git are staged and can then be excluded.
        Copy-Item -LiteralPath $SourceFolder -Destination $tempDir -Recurse -Force -ErrorAction Stop

        Write-Verbose "Removing excluded items..."
        foreach ($pattern in $Exclude) {
            Get-ChildItem -LiteralPath $tempDir -Recurse -Filter $pattern -Force -ErrorAction SilentlyContinue |
                Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
        }

        if (Test-Path -LiteralPath $OutputFile) {
            Remove-Item -LiteralPath $OutputFile -Force
        }

        Write-Verbose "Creating archive..."
        Compress-Archive -Path (Join-Path $tempDir "*") -DestinationPath $OutputFile -Force -ErrorAction Stop

        Write-Host ("Created archive: {0}" -f (Get-Hyperlink -Path $OutputFile)) -ForegroundColor Green
    }
    finally {
        if (Test-Path -LiteralPath $tempDir) {
            Remove-Item -LiteralPath $tempDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}
