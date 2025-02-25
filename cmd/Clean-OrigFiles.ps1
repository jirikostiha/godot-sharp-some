[CmdletBinding(SupportsShouldProcess)]
param()

# Set the path to the upper directory
$Root = (Get-Item .).Parent.FullName

# Find and remove all .orig files
Get-ChildItem -Path $Root -Recurse -File -Filter *.orig | ForEach-Object {
    Write-Host "Removing file: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Force -Path $_.FullName
}