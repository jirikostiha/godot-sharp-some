[CmdletBinding(SupportsShouldProcess)]
param()

# Set the path to the upper directory
$Root = (Get-Item .).Parent.FullName

# Find and remove all 'obj' folders
Get-ChildItem -Path $Root -Recurse -Directory -Filter obj | ForEach-Object {
    Write-Host "Removing folder: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Force -Recurse -Path $_.FullName
}

# Find and remove all 'bin' folders
Get-ChildItem -Path $Root -Recurse -Directory -Filter bin | ForEach-Object {
    Write-Host "Removing folder: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Force -Recurse -Path $_.FullName
}

# Find and remove all 'asm' folders
Get-ChildItem -Path $Root -Recurse -Directory -Filter asm | ForEach-Object {
    Write-Host "Removing folder: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Force -Recurse -Path $_.FullName
}