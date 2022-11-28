param(
    [switch] $Minor
)

$scriptDescription = "update product version, commit"
$scriptVersion = "1.2"
$svn = "git"

$versionFile = "..\product_version.props"

write-host "updating product version.. " -NoNewline; write-verbose "(script version: $scriptVersion)"
[xml]$versionFileXml = Get-Content $versionFile
$oldVersion = New-Object System.Version($versionFileXml.Project.PropertyGroup.VersionPrefix)
if ($Minor) {
    $newVersion = New-Object System.Version($oldVersion.Major, ($oldVersion.Minor + 1), 0)
}
else {
    $newVersion = New-Object System.Version($oldVersion.Major, $oldVersion.Minor, ($oldVersion.Build + 1))
}

"$oldVersion -> $newVersion"
$versionFileXml.Project.PropertyGroup.VersionPrefix = $newVersion.ToString()

$versionFileXml.Save($versionFile)

"publishing product version.."
git add $versionFile
$message = "product: update version $oldVersion -> $newVersion"
git commit -m $message