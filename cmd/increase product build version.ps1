$scriptDescription = "update product build version, commit"
$scriptVersion = "1.0"
$svn = "git"

$versionFile = "..\product_version.props"

write "increasing product version.."
[xml]$versionFileXml = Get-Content $versionFile
$oldVersion = New-Object System.Version($versionFileXml.Project.PropertyGroup.VersionPrefix)
$newVersion = New-Object System.Version($oldVersion.Major, $oldVersion.Minor, ($oldVersion.Build + 1))
write "$($oldVersion.ToString()) -> $($newVersion.ToString())"
$versionFileXml.Project.PropertyGroup.VersionPrefix = $newVersion.ToString()

$versionFileXml.Save($versionFile)

write "publishing product version.."
git add $versionFile
git commit -m "product: increase version"
git push