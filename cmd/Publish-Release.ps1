<#
.SYNOPSIS
Runs the whole release pipeline: build, test, lint, version, tag and push.

.DESCRIPTION
Orchestrates the existing scripts in this folder in one pass:

  1. Build-Sln.ps1 and Test-Sln.ps1 - a failure stops the run. A repository with
     no solution file skips the build and tests its projects directly with
     Test-Csprojs.ps1, the default runner.
  2. Lint-Code.ps1 - formatting fixes are committed.
  3. Other linters that are installed (PSScriptAnalyzer, markdownlint-cli2) -
     auto-fixable findings are applied and committed, the rest are reported.
  4. The product version is set from the branch type (see below).
  5. Commit-Version.ps1 commits the version file.
  6. Tag-Commit.ps1 tags the commit.
  7. A clean release on a release branch is merged into main, and the merge commit sets
     main's next dev version (VER-03). -NoMerge suppresses it. A merge that cannot be made
     is reported, the release stays as it is, and the script exits with code 7 (Git).
  8. The branch and the tags are pushed, and main too when the release was cut from it
     or merged into it.
     A push that fails - an unreachable remote, for example - does not stop the run:
     every local step is kept, the remaining pushes are still tried, the failed ones are
     listed as commands to retry, and the script exits with code 7 (Git).

Version rules by branch:

  -Version            Sets the numeric part outright; the suffix still follows the
                      rules below.
  rel/X.Y[.Z] or      The suffix is empty (a clean release; dev below 1.0). A v before
  release/X.Y[.Z]     the version is accepted; any other name under rel/ or release/ is
                      refused before the build. The patch part is raised until the version outranks every version tag reachable
                      from HEAD, so a fresh release branch keeps its prefix and a
                      re-run moves to the next patch.
  anything else       The suffix is dev and the minor part is raised until the
                      version is above the last release tag and above every
                      version tag reachable from HEAD.

On main the run does not build a dev version: it cuts a release. Steps 1-3 run on main
first, so a failing build, test or lint leaves no branch and no version bump behind, and
any lint fixes are committed to main. rel/X.Y is then opened at that commit, with X.Y
taken from -Version or, without it, from the minor main carries, and the release proceeds
on that branch. When the X.Y line already exists - under any release-branch name, such as
rel/X.Y, release/X.Y, release/vX.Y or rel/X.Y.Z, or, without -Version, as a release tag
vX.Y.Z made straight on main - main still holds a version that has been taken, so main is
first advanced with -dev to the first line above X.Y that no branch or tag has released yet
(reusing Set-ProductVersion and Commit-Version), and that line is cut instead.

Below 1.0 (major 0) there is no release branch and no clean release. The version is always
suffixed dev, on any branch; on main the run stays on main, bumps the version by the rules of
"anything else" above and tags it, so v0.6.0-dev is followed by v0.7.0-dev. Whether the
release is below 1.0 is judged from -Version when it is given, otherwise from the version
main carries, so -Version 1.0.0 on main carrying 0.9.0-dev cuts rel/1.0 as usual.

The version is read from <repo root>\product_version.props. When that file does
not exist, the projects listed in the solution are searched instead and every
project carrying a version is updated; they must all carry the same version.

The working tree must be clean before the run, because steps 2 and 3 commit
whatever they changed.

.PARAMETER Solution
An explicit solution path, overriding discovery. Also used to find the projects
carrying the version when there is no product_version.props.

.PARAMETER VersionFile
An explicit path to the version file, overriding discovery.

.PARAMETER Version
An explicit version to release, numbers only, such as 2.5.0. It is written to the
version file as given and the tag follows it, with the branch name not consulted
and no part raised to clear existing tags. The suffix still follows the branch: a
release branch produces a clean release, any other branch a dev build. A version
whose tag already exists is refused.

.PARAMETER Configuration
The build configuration for the build and the tests. Defaults to Release.

.PARAMETER NoPush
Performs everything locally and leaves the commits and the tag unpushed.

.PARAMETER NoMerge
Leaves main alone after a release: the release branch is not merged into it. The merge,
with main's next dev version set in the merge commit, is then done by hand.

.EXAMPLE
.\Publish-Release.ps1
Builds, tests, lints, bumps, commits, tags and pushes with the branch defaults.

.EXAMPLE
.\Publish-Release.ps1 -Version 2.5.0
On a release branch, releases exactly 2.5.0: the version file is set to it and the
tag becomes v2.5.0.

.EXAMPLE
.\Publish-Release.ps1
On main carrying 4.2.0-dev, cuts rel/4.2 at the current commit and releases 4.2.0. If
rel/4.2 or tag v4.2.0 already exists, advances main to 4.3.0-dev (or past any later line
already released), cuts rel/4.3 and releases 4.3.0.

.EXAMPLE
.\Publish-Release.ps1 -NoMerge
On rel/4.2, releases 4.2.x but does not merge rel/4.2 into main.

.EXAMPLE
.\Publish-Release.ps1 -Verbose
Reports how each version decision was reached and passes -Verbose on to the
build, test and lint scripts, which run as their own processes.

.EXAMPLE
.\Publish-Release.ps1 -WhatIf
Builds, tests and verifies formatting, and reports the version, the tag and the
push it would have made without changing anything.
#>
<#---
name: Publish-Release
kind: cmd
description: Runs the release pipeline end to end - build, test, lint, version bump by branch type, commit, tag, merge into main and push. Use to cut a release.
profiles: [dotnet]
version: 3.4.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Solution,
    [string] $VersionFile = (Join-Path $PSScriptRoot ".." "product_version.props"),
    [ValidatePattern('(?i)^v?\d+(\.\d+){1,3}$')]
    [string] $Version,
    [string] $Configuration = "Release",
    [switch] $NoPush,
    [switch] $NoMerge
)

. (Join-Path $PSScriptRoot "Common.ps1")

$ErrorActionPreference = "Stop"
# Native exit codes are checked explicitly below; letting the preference throw on
# any stderr write would turn ordinary git progress output into a failure.
$PSNativeCommandUseErrorActionPreference = $false

# A release branch is rel/X.Y or release/X.Y, optionally with a v and a patch part
# (release/v2.4, rel/2.4.1). Groups: major, minor, patch. [0-9] rather than \d, which
# would also accept non-ASCII digits that [version] and [int] then reject. Any other
# name under the prefix is refused up front rather than released under a version its
# name does not state.
$ReleaseBranchPrefix = '^(?:rel|release)/'
$ReleaseBranchPattern = $ReleaseBranchPrefix + 'v?([0-9]+)\.([0-9]+)(?:\.([0-9]+))?$'

function Write-Step {
    <#
    .SYNOPSIS
    Writes a step header so the log of a long run stays readable.

    .PARAMETER Text
    The step title.

    .EXAMPLE
    Write-Step "Build and test"
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $Text
    )

    Write-Host ""
    Write-Host ("== {0}" -f $Text) -ForegroundColor Cyan
}

function Invoke-Step {
    <#
    .SYNOPSIS
    Runs a cmd script as a child process and throws with its reason on a non-zero exit.

    .DESCRIPTION
    The build, test and lint scripts end with a standardized exit code (see Get-AiKitExitCode)
    rather than a thrown error, so they must run as their own process — the call operator would
    let their exit tear down this pipeline. This runs one such script and turns a non-zero exit
    into a throw that the pipeline's own handling reports.

    .PARAMETER Script
    The script file name, resolved beside this one.

    .PARAMETER Arguments
    The arguments to pass through.

    .PARAMETER FailMessage
    The message to throw on a non-zero exit.

    .EXAMPLE
    Invoke-Step -Script "Build-Sln.ps1" -Arguments @("-Configuration", "Release") -FailMessage "Build failed."
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)] [string] $Script,
        [string[]] $Arguments = @(),
        [Parameter(Mandatory)] [string] $FailMessage
    )

    # A child process inherits no preference variables, so -Verbose has to be passed on
    # explicitly or the called script stays silent about how it decided.
    $childArguments = @($Arguments)
    if ($VerbosePreference -ne [System.Management.Automation.ActionPreference]::SilentlyContinue) {
        $childArguments += "-Verbose"
    }

    Write-Verbose ("Running {0} {1}" -f $Script, ($childArguments -join " "))
    pwsh -NoProfile -File (Join-Path $PSScriptRoot $Script) @childArguments
    if ($LASTEXITCODE -ne 0) {
        throw ("{0} (exit {1})." -f $FailMessage, $LASTEXITCODE)
    }
}

function Invoke-Git {
    <#
    .SYNOPSIS
    Runs git and returns its output, throwing on a non-zero exit code.

    .PARAMETER Arguments
    The git arguments, one array element per argument.

    .PARAMETER AllowFailure
    Returns the output of a failed call instead of throwing.

    .EXAMPLE
    Invoke-Git -Arguments "rev-parse", "--abbrev-ref", "HEAD"
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [string[]] $Arguments,

        [switch] $AllowFailure
    )

    Write-Verbose ("git {0}" -f ($Arguments -join " "))
    $output = @(& git @Arguments 2>&1 | ForEach-Object { "$_" })

    if ($LASTEXITCODE -ne 0 -and -not $AllowFailure) {
        throw ("git {0} failed (exit code: {1}).{2}{3}" -f ($Arguments -join " "), $LASTEXITCODE, [Environment]::NewLine, ($output -join [Environment]::NewLine))
    }

    # Call sites that index or count wrap the result in @(), because a single output
    # line arrives unrolled as one string.
    return $output
}

function Get-SolutionProjectPath {
    <#
    .SYNOPSIS
    Gets the full paths of the projects referenced by a solution file.

    .DESCRIPTION
    Understands both layouts: the XML of a .slnx and the Project lines of a .sln.
    Only project files are returned; solution folders and files are skipped.

    .PARAMETER Path
    The solution file to read.

    .EXAMPLE
    Get-SolutionProjectPath -Path .\src\app.slnx
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $Path
    )

    $folder = Split-Path -Parent $Path
    $extensions = @(".csproj", ".fsproj", ".vbproj")
    $relative = @()

    if ([IO.Path]::GetExtension($Path) -eq ".slnx") {
        $xml = [xml](Get-Content -LiteralPath $Path -Raw -ErrorAction Stop)
        $relative = @($xml.SelectNodes("//Project[@Path]") | ForEach-Object { $_.GetAttribute("Path") })
    }
    else {
        $content = Get-Content -LiteralPath $Path -Raw -ErrorAction Stop
        $relative = @([regex]::Matches($content, '(?m)^Project\("\{[^}]+\}"\)\s*=\s*"[^"]*",\s*"([^"]+)"') |
                ForEach-Object { $_.Groups[1].Value })
    }

    $relative |
        Where-Object { [IO.Path]::GetExtension($_) -in $extensions } |
        ForEach-Object { [IO.Path]::GetFullPath((Join-Path $folder ($_ -replace "\\", [IO.Path]::DirectorySeparatorChar))) } |
        Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } |
        Sort-Object -Unique
}

function Get-VersionCarrierPath {
    <#
    .SYNOPSIS
    Gets the files that carry the product version.

    .DESCRIPTION
    Resolution order: the explicit path, then product_version.props in the
    repository root, then the projects of the solution that declare a version.
    The last case may yield several files; they are then kept in step.

    .PARAMETER VersionFile
    An explicit path, absolute or relative to the repository root.

    .PARAMETER Solution
    An explicit solution path used for the project fallback.

    .EXAMPLE
    Get-VersionCarrierPath
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    param(
        [string] $VersionFile,
        [string] $Solution
    )

    if (-not [string]::IsNullOrWhiteSpace($VersionFile)) {
        return @(Get-VersionFile -Path $VersionFile)
    }

    # Honour .aikit.json config and the product_version.props convention via the
    # canonical resolver. Fall through to the multi-project path only when no
    # single file is identified (multiple carriers, or none at all).
    try {
        return @(Get-VersionFile)
    }
    catch { }

    $slnPath = Get-SolutionPath -Path $Solution
    $carriers = @(Get-SolutionProjectPath -Path $slnPath | Where-Object { Test-VersionCarrier -Path $_ })

    if ($carriers.Count -eq 0) {
        throw ("No version file found and no project in '{0}' declares a version. Pass -VersionFile or set repo.versionFile in .aikit.json." -f $slnPath)
    }

    return $carriers
}

function Get-StageRank {
    <#
    .SYNOPSIS
    Ranks a version suffix so two versions with the same prefix can be ordered.

    .DESCRIPTION
    A release outranks a development build: 2.4.0-dev < 2.4.0. A legacy release
    candidate tag still sorts between them (2.4.0-dev < 2.4.0-rc < 2.4.0) so
    comparisons against old history stay correct; the pipeline no longer emits rc.
    Unknown suffixes rank as development builds, the conservative end.

    .PARAMETER Suffix
    The version suffix: dev, an empty string for a release, or a legacy rc/rel.

    .EXAMPLE
    Get-StageRank -Suffix "rc"
    #>
    [CmdletBinding()]
    [OutputType([int])]
    param(
        [Parameter(Position = 0)]
        [AllowEmptyString()]
        [string] $Suffix
    )

    if ([string]::IsNullOrWhiteSpace($Suffix)) { return 2 }

    switch ($Suffix.Trim().ToLowerInvariant()) {
        "rel" { return 2 }
        "rc" { return 1 }
        default { return 0 }
    }
}

function ConvertFrom-VersionTag {
    <#
    .SYNOPSIS
    Parses a version tag such as v2.4.0-rc into its prefix and suffix.

    .DESCRIPTION
    Yields nothing for anything that is not a version tag, so the whole tag list
    can be piped through it.

    .PARAMETER Tag
    The tag name.

    .EXAMPLE
    git tag --list | ConvertFrom-VersionTag
    #>
    [CmdletBinding()]
    [OutputType([pscustomobject])]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline)]
        [AllowEmptyString()]
        [string] $Tag
    )

    process {
        if ([string]::IsNullOrWhiteSpace($Tag)) { return }

        $match = [regex]::Match($Tag.Trim(), '^v?(\d+(?:\.\d+){1,3})(?:-(.+))?$')
        if (-not $match.Success) { return }

        [pscustomobject]@{
            Tag    = $Tag.Trim()
            Prefix = ConvertTo-ThreePartVersion ([version] $match.Groups[1].Value)
            Suffix = $match.Groups[2].Value
            Rank   = Get-StageRank -Suffix $match.Groups[2].Value
        }
    }
}

function Test-VersionOutranked {
    <#
    .SYNOPSIS
    Determines whether a candidate version fails to beat every given tag.

    .DESCRIPTION
    True when a tag carries a higher version, or the same version at the same or
    a later stage. 2.4.0 beats 2.4.0-rc, 2.4.0-rc does not beat 2.4.0-rc.

    .PARAMETER Prefix
    The candidate version prefix.

    .PARAMETER Suffix
    The candidate version suffix.

    .PARAMETER Tag
    The parsed tags to compare against.

    .EXAMPLE
    Test-VersionOutranked -Prefix ([version]"2.4.0") -Suffix "rc" -Tag $tags
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory)]
        [version] $Prefix,

        [Parameter(Mandatory)]
        [AllowEmptyString()]
        [string] $Suffix,

        [pscustomobject[]] $Tag
    )

    $rank = Get-StageRank -Suffix $Suffix

    foreach ($candidate in @($Tag)) {
        if ($null -eq $candidate) { continue }
        if ($candidate.Prefix -gt $Prefix) { return $true }
        if ($candidate.Prefix -eq $Prefix -and $candidate.Rank -ge $rank) { return $true }
    }

    return $false
}

function Test-TagExists {
    <#
    .SYNOPSIS
    Determines whether an exact version tag already exists in a set.

    .DESCRIPTION
    Symmetric to Test-VersionOutranked: where that function checks whether a tag
    outranks the candidate, this one checks for an exact prefix-and-suffix match.
    Used to prevent creating a tag that already exists on a different branch.

    .PARAMETER Prefix
    The version prefix to look for.

    .PARAMETER Suffix
    The version suffix to look for.

    .PARAMETER Tag
    The parsed tags to search.

    .EXAMPLE
    Test-TagExists -Prefix ([version]"2.4.0") -Suffix "rc" -Tag $allTags
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory)]
        [version] $Prefix,

        [Parameter(Mandatory)]
        [AllowEmptyString()]
        [string] $Suffix,

        [pscustomobject[]] $Tag
    )

    return [bool](@($Tag) | Where-Object { $null -ne $_ -and $_.Prefix -eq $Prefix -and $_.Suffix -eq $Suffix } | Select-Object -First 1)
}

function Get-TargetVersion {
    <#
    .SYNOPSIS
    Computes the version this branch must carry.

    .DESCRIPTION
    Implements the branch rules described in the script help: patch steps with an
    empty suffix on a release branch, minor steps with a dev suffix everywhere
    else. The part is raised until the result beats the relevant tags, so running
    the pipeline twice cannot produce the same tag.

    .PARAMETER Branch
    The current branch name.

    .PARAMETER Current
    The version currently in the version file, as returned by Get-ProductVersion.

    .EXAMPLE
    Get-TargetVersion -Branch "rel/2.4.0" -Current $current
    #>
    [CmdletBinding()]
    [OutputType([pscustomobject])]
    param(
        [Parameter(Mandatory)]
        [string] $Branch,

        [Parameter(Mandatory)]
        [pscustomobject] $Current,

        [string] $Version
    )

    $allTags = @(Invoke-Git -Arguments "tag", "--list" | ConvertFrom-VersionTag)
    # Derive reachable from allTags by name to avoid re-running ConvertFrom-VersionTag.
    $allTagsByName = @{}
    foreach ($t in $allTags) { if ($t) { $allTagsByName[$t.Tag] = $t } }
    $reachable = @(Invoke-Git -Arguments "tag", "--merged", "HEAD" |
        Where-Object { $_ -and $allTagsByName.ContainsKey($_) } |
        ForEach-Object { $allTagsByName[$_] })

    $prefix = ConvertTo-ThreePartVersion $Current.Prefix

    # An explicit version is an operator decision: it settles the prefix outright, and its
    # suffix - when it carries one - settles the stage too.
    $isExplicit = -not [string]::IsNullOrWhiteSpace($Version)
    if ($isExplicit) {
        # Numbers only. The suffix is the stage's business, so accepting one here would
        # give two ways to say the same thing and let them disagree.
        $parsed = [regex]::Match($Version.Trim(), '^v?(\d+(?:\.\d+){1,3})$')
        if (-not $parsed.Success) {
            throw ("'{0}' is not a version. Expected numbers only, such as 2.5.0; the suffix comes from the branch." -f $Version)
        }

        $prefix = ConvertTo-ThreePartVersion ([version] $parsed.Groups[1].Value)
        Write-Verbose ("Version {0} requested explicitly; only the suffix is still decided." -f $prefix)
    }

    $named = [regex]::Match($Branch, $ReleaseBranchPattern)
    if ($named.Success) {
        # A release branch names the version it prepares; honour that name when the
        # file still carries the lower version inherited from main. An explicit version
        # outranks the branch name - it was typed for this run.
        if (-not $isExplicit) {
            # An absent patch group is an empty string, which [int] turns into 0.
            $branchVersion = [version] ("{0}.{1}.{2}" -f $named.Groups[1].Value, $named.Groups[2].Value, [int] $named.Groups[3].Value)
            if ($branchVersion -gt $prefix) { $prefix = $branchVersion }
        }

        # A release branch ships a clean, unsuffixed release - except below 1.0, where every
        # version stays dev.
        $suffix = if ($prefix.Major -eq 0) { "dev" } else { "" }

        $nextPrefix = { param($p) [version] ("{0}.{1}.{2}" -f $p.Major, $p.Minor, ($p.Build + 1)) }
    }
    else {
        $suffix = "dev"

        # main stays above the last release, which usually lives on a release
        # branch and is therefore not reachable from here.
        $lastRelease = @($allTags | Where-Object { $_.Rank -eq 2 } | Sort-Object Prefix | Select-Object -Last 1)
        if (-not $isExplicit -and $lastRelease.Count -eq 1) {
            $floor = ConvertTo-NextMinor $lastRelease[0].Prefix
            if ($floor -gt $prefix) { $prefix = $floor }
        }

        $nextPrefix = { param($p) ConvertTo-NextMinor $p }
    }

    Write-Verbose ("Tags: {0} in the repository, {1} reachable from HEAD." -f $allTags.Count, $reachable.Count)
    Write-Verbose ("Starting from {0} with suffix '{1}'." -f $prefix, $suffix)

    if ($isExplicit) {
        # Silently bumping past what was asked for would defeat the point, so an
        # already-taken tag is refused instead.
        if (Test-TagExists -Prefix $prefix -Suffix $suffix -Tag $allTags) {
            $taken = if ($suffix) { "v$prefix-$suffix" } else { "v$prefix" }
            throw ("Tag {0} already exists. Pass a different -Version." -f $taken)
        }
    }
    else {
        while ((Test-VersionOutranked -Prefix $prefix -Suffix $suffix -Tag $reachable) -or
            (Test-TagExists -Prefix $prefix -Suffix $suffix -Tag $allTags)) {
            $bumped = & $nextPrefix $prefix
            Write-Verbose ("{0} is taken or outranked; trying {1}." -f $prefix, $bumped)
            $prefix = $bumped
        }
    }

    [pscustomobject]@{
        Prefix  = $prefix
        Suffix  = $suffix
        Stage   = if ($suffix) { $suffix } else { "final" }
        Display = if ($suffix) { "$prefix-$suffix" } else { "$prefix" }
        Tag     = if ($suffix) { "v$prefix-$suffix" } else { "v$prefix" }
    }
}

function Get-ReleaseLine {
    <#
    .SYNOPSIS
    Picks the MAJOR.MINOR line a release cut from main should target.

    .DESCRIPTION
    From -Version when it is given (its major and minor), otherwise from the version
    main currently carries. The patch and the suffix are the release branch's business,
    so only the two leading parts are returned.

    .PARAMETER Current
    The version currently in the version file, as returned by Get-ProductVersion.

    .PARAMETER Version
    The explicit -Version, if any.

    .EXAMPLE
    Get-ReleaseLine -Current $current
    #>
    [CmdletBinding()]
    [OutputType([pscustomobject])]
    param(
        [Parameter(Mandatory)]
        [pscustomobject] $Current,

        [string] $Version
    )

    if (-not [string]::IsNullOrWhiteSpace($Version)) {
        $m = [regex]::Match($Version.Trim(), '^v?(\d+)\.(\d+)')
        if (-not $m.Success) {
            throw ("'{0}' is not a version; expected at least MAJOR.MINOR, such as 4.2." -f $Version)
        }
        return [pscustomobject]@{ Major = [int] $m.Groups[1].Value; Minor = [int] $m.Groups[2].Value }
    }

    $prefix = ConvertTo-ThreePartVersion $Current.Prefix
    return [pscustomobject]@{ Major = $prefix.Major; Minor = $prefix.Minor }
}

function Find-ReleaseBranch {
    <#
    .SYNOPSIS
    Finds the existing release branch of a MAJOR.MINOR line, whatever form its name takes.

    .DESCRIPTION
    A new line is always cut as rel/X.Y, but a line already carried by release/X.Y,
    rel/vX.Y or rel/X.Y.Z is the same line, and missing it would cut a duplicate beside
    it. Looks at local branches, remote-tracking branches and the remote itself, for a
    branch pushed elsewhere but not yet fetched. An unreachable remote is tolerated: the
    local checks still stand. Returns the first matching name, or nothing when the line
    is still free.

    .PARAMETER Major
    Major part of the release line.

    .PARAMETER Minor
    Minor part of the release line.

    .EXAMPLE
    Find-ReleaseBranch -Major 4 -Minor 2
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [int] $Major,

        [Parameter(Mandatory)]
        [int] $Minor
    )

    # Takes full ref names from either source and returns the first branch of the line.
    $selectLine = {
        param($refs)
        $refs | ForEach-Object { ($_ -split '\s+')[-1] -replace '^refs/(heads|remotes/origin)/', '' } |
            Where-Object {
                $m = [regex]::Match($_, $ReleaseBranchPattern)
                $m.Success -and [int] $m.Groups[1].Value -eq $Major -and [int] $m.Groups[2].Value -eq $Minor
            } |
            Sort-Object | Select-Object -First 1
    }

    $local = & $selectLine (Invoke-Git -Arguments "for-each-ref", "--format=%(refname)",
        "refs/heads/rel", "refs/heads/release", "refs/remotes/origin/rel", "refs/remotes/origin/release")
    if ($local) { return $local }

    $remote = @(Invoke-Git -Arguments "ls-remote", "--heads", "origin", "rel/*", "release/*" -AllowFailure)
    if ($LASTEXITCODE -ne 0) {
        Write-Warning ("The remote could not be checked for release line {0}.{1} (git exit code: {2}); relying on local refs only." -f $Major, $Minor, $LASTEXITCODE)
        return $null
    }
    return (& $selectLine $remote)
}

function Find-ReleaseTag {
    <#
    .SYNOPSIS
    Finds the newest release tag of a MAJOR.MINOR line.

    .DESCRIPTION
    A line released straight from main, before release branches were cut, has a clean
    release tag such as v4.2.0 but no branch, so Find-ReleaseBranch misses it. Development
    tags (v4.2.0-dev) do not count: they do not release the line. Returns the tag name, or
    nothing when no release of the line was tagged.

    .PARAMETER Major
    Major part of the release line.

    .PARAMETER Minor
    Minor part of the release line.

    .EXAMPLE
    Find-ReleaseTag -Major 4 -Minor 2
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [int] $Major,

        [Parameter(Mandatory)]
        [int] $Minor
    )

    return (Invoke-Git -Arguments "tag", "--list" | ConvertFrom-VersionTag |
            Where-Object { $_.Rank -eq 2 -and $_.Prefix.Major -eq $Major -and $_.Prefix.Minor -eq $Minor } |
            Sort-Object Prefix | Select-Object -Last 1 | ForEach-Object { $_.Tag })
}

function Find-ReleaseLine {
    <#
    .SYNOPSIS
    Finds what already releases a MAJOR.MINOR line: a release branch or a release tag.

    .DESCRIPTION
    Returns the branch name from Find-ReleaseBranch or, when there is none, the tag name
    from Find-ReleaseTag; nothing when the line is still free.

    .PARAMETER Major
    Major part of the release line.

    .PARAMETER Minor
    Minor part of the release line.

    .EXAMPLE
    Find-ReleaseLine -Major 4 -Minor 2
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [int] $Major,

        [Parameter(Mandatory)]
        [int] $Minor
    )

    $branch = Find-ReleaseBranch -Major $Major -Minor $Minor
    if ($branch) { return $branch }
    return (Find-ReleaseTag -Major $Major -Minor $Minor)
}

function Test-Git {
    <#
    .SYNOPSIS
    Runs git for its exit code alone: returns $true when it succeeded.

    .PARAMETER Arguments
    The git arguments, one array element per argument.

    .EXAMPLE
    Test-Git "merge-base", "--is-ancestor", "main", "origin/main"
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [string[]] $Arguments
    )

    Invoke-Git -Arguments $Arguments -AllowFailure | Out-Null
    return $LASTEXITCODE -eq 0
}

function ConvertTo-NextMinor {
    <#
    .SYNOPSIS
    Returns the first version of the next minor line: 2.4.1 -> 2.5.0.

    .PARAMETER Prefix
    The version to advance.

    .EXAMPLE
    ConvertTo-NextMinor -Prefix ([version] "2.4.1")
    #>
    [CmdletBinding()]
    [OutputType([version])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [version] $Prefix
    )

    return [version] ("{0}.{1}.0" -f $Prefix.Major, ($Prefix.Minor + 1))
}

function Invoke-GitPush {
    <#
    .SYNOPSIS
    Pushes to the remote and reports a failure as a warning instead of throwing.

    .DESCRIPTION
    Everything before the push is local and already done, so an unreachable remote must
    not throw it away. Returns $true when the push succeeded and $false otherwise, so the
    caller can list the failed push for a retry.

    .PARAMETER Arguments
    The arguments after "git push", one array element per argument.

    .EXAMPLE
    Invoke-GitPush -Arguments "origin", "--tags"
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [string[]] $Arguments
    )

    $output = @(Invoke-Git -Arguments (@("push") + $Arguments) -AllowFailure)
    if ($LASTEXITCODE -eq 0) { return $true }

    Write-Warning ("git push {0} failed (exit code: {1}).{2}{3}" -f ($Arguments -join " "), $LASTEXITCODE, [Environment]::NewLine, ($output -join [Environment]::NewLine))
    return $false
}

function Merge-ReleaseIntoMain {
    <#
    .SYNOPSIS
    Merges the release branch into main; the merge commit itself sets main to its next dev version.

    .DESCRIPTION
    main carries the next version with -dev (VER-03). That version is a helper, not a release,
    so it never gets a commit of its own: the merge commit that brings the release into main
    sets it. It is the next minor above the release, or the version main already carries when
    that is higher, so a patch release of an older line never lowers main.

    The release is already committed and tagged when this runs, so a merge that cannot be made
    is reported, never thrown: the merge is aborted, the release branch is checked out again and
    Failed is returned. The same holds when there is no main at all or main has diverged from
    origin/main. A local main that is only behind origin/main is fast-forwarded first, so the merge
    does not land on a stale main, and a missing local main is created from origin/main.
    Returns Merged, UpToDate when main already contains the release, Skipped under -WhatIf,
    or Failed.

    .PARAMETER Branch
    The release branch to merge, checked out when this is called.

    .PARAMETER Release
    The released version: an object with a [version] Prefix.

    .PARAMETER Carriers
    The files carrying the product version.

    .EXAMPLE
    Merge-ReleaseIntoMain -Branch "rel/4.2" -Release $target -Carriers $carriers
    #>
    [CmdletBinding(SupportsShouldProcess)]
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [string] $Branch,

        [Parameter(Mandatory)]
        [object] $Release,

        [Parameter(Mandatory)]
        [string[]] $Carriers
    )

    $next = ConvertTo-NextMinor $Release.Prefix

    if (-not $PSCmdlet.ShouldProcess("main", ("Merge {0} and set the version to {1}-dev, or keep a higher one main carries" -f $Branch, $next))) {
        return "Skipped"
    }

    # An unreachable remote is tolerated here as for the push: the local main is merged as it is.
    Invoke-Git -Arguments "fetch", "--quiet", "origin", "main" -AllowFailure | Out-Null
    $hasRemoteMain = Test-Git "rev-parse", "--verify", "--quiet", "refs/remotes/origin/main"
    $hasLocalMain = Test-Git "rev-parse", "--verify", "--quiet", "refs/heads/main"
    if ($hasRemoteMain) {
        if (-not $hasLocalMain -or (Test-Git "merge-base", "--is-ancestor", "main", "origin/main")) {
            # Creates a main the clone never checked out, or fast-forwards a stale one; main is
            # not checked out, so moving the ref is all it takes.
            Invoke-Git -Arguments "branch", "--force", "--track", "main", "origin/main" | Out-Null
        }
        elseif (-not (Test-Git "merge-base", "--is-ancestor", "origin/main", "main")) {
            Write-Warning "main has diverged from origin/main; bring them together first."
            return "Failed"
        }
    }
    elseif (-not $hasLocalMain) {
        Write-Warning "There is no main branch to merge the release into."
        return "Failed"
    }

    if (Test-Git "merge-base", "--is-ancestor", $Branch, "main") {
        Write-Host ("  main already contains {0}; nothing to merge." -f $Branch) -ForegroundColor DarkGray
        return "UpToDate"
    }

    Invoke-Git -Arguments "checkout", "--quiet", "main" | Out-Null
    try {
        $mainVersion = Get-ProductVersion -Path $Carriers[0]
        $prefix = if ($mainVersion.Prefix -gt $next) { $mainVersion.Prefix } else { $next }

        $mergeOutput = @(Invoke-Git -Arguments "merge", "--no-ff", "--no-commit", $Branch -AllowFailure)
        # A merge git refused outright leaves no MERGE_HEAD, and committing then would record a
        # plain version bump instead of the merge.
        if (-not (Test-Git "rev-parse", "--verify", "--quiet", "MERGE_HEAD")) {
            Write-Warning ("Merging {0} into main failed.{1}{2}" -f $Branch, [Environment]::NewLine, ($mergeOutput -join [Environment]::NewLine))
            return "Failed"
        }
        $conflicts = @(Invoke-Git -Arguments "diff", "--name-only", "--diff-filter=U" | Where-Object { $_ })
        # git reports paths relative to the repository root.
        $carrierPaths = @($Carriers | ForEach-Object { [IO.Path]::GetRelativePath($root, $_) -replace '\\', '/' })
        $otherConflicts = @($conflicts | Where-Object { $carrierPaths -notcontains $_ })
        if ($otherConflicts.Count -gt 0) {
            Invoke-Git -Arguments "merge", "--abort" | Out-Null
            Write-Warning ("Merging {0} into main conflicts in: {1}. Merge it by hand; the merge commit sets main to {2}-dev." -f $Branch, ($otherConflicts -join ", "), $prefix)
            return "Failed"
        }

        # The version file is the one conflict a patch release always brings. Whatever either side
        # holds is replaced below, so main's side is taken only to give the file valid content again.
        if ($conflicts.Count -gt 0) {
            Invoke-Git -Arguments (@("checkout", "--ours", "--") + $conflicts) | Out-Null
        }
        foreach ($carrier in $Carriers) {
            Set-ProductVersion -Path $carrier -Prefix $prefix -Suffix dev -Confirm:$false
        }
        Invoke-Git -Arguments (@("add", "--") + $Carriers) | Out-Null
        Invoke-Git -Arguments "commit", "--quiet", "--no-edit" | Out-Null
        Write-Host ("  Merged {0} into main; main now carries {1}-dev." -f $Branch, $prefix) -ForegroundColor Green
        return "Merged"
    }
    finally {
        Invoke-Git -Arguments "checkout", "--quiet", $Branch | Out-Null
    }
}

function Save-WorkingTreeChange {
    <#
    .SYNOPSIS
    Commits everything the previous step changed, if anything.

    .DESCRIPTION
    Returns $true when a commit was made. Nothing to commit is the normal case
    and is not an error.

    .PARAMETER Message
    The commit message, which must follow the conventional commit format.

    .EXAMPLE
    Save-WorkingTreeChange -Message "style: apply code formatting"
    #>
    [CmdletBinding(SupportsShouldProcess)]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $Message
    )

    $changes = @(Invoke-Git -Arguments "status", "--porcelain" | Where-Object { $_ })
    if ($changes.Count -eq 0) {
        Write-Host "  Nothing to commit." -ForegroundColor DarkGray
        return $false
    }

    Write-Host ("  {0} file(s) changed." -f $changes.Count) -ForegroundColor Yellow
    if (-not $PSCmdlet.ShouldProcess($Message, "git commit")) { return $false }

    Invoke-Git -Arguments "add", "--all" | Out-Null
    Invoke-Git -Arguments "commit", "-m", $Message | Out-Null
    Write-Host ("  Committed: {0}" -f $Message) -ForegroundColor Green

    return $true
}

function Invoke-OtherLint {
    <#
    .SYNOPSIS
    Runs the non-compiler linters that are installed, fixing what they can fix.

    .DESCRIPTION
    PSScriptAnalyzer covers the scripts and markdownlint-cli2 the documentation.
    A linter that is not installed is reported and skipped, so the pipeline does
    not depend on optional tooling. Findings that no linter can fix are reported
    as warnings and do not stop the release. Under -WhatIf the linters only
    report, because their fixes rewrite files.

    .EXAMPLE
    Invoke-OtherLint
    #>
    [CmdletBinding()]
    param()

    $root = Get-RepositoryRoot
    $fix = -not $WhatIfPreference

    $hasPsa = try { Import-Module PSScriptAnalyzer -ErrorAction Stop; $true } catch { $false }
    if ($hasPsa) {
        Write-Host ("  PSScriptAnalyzer: {0} ..." -f $(if ($fix) { "fixing what is fixable" } else { "reporting only" })) -ForegroundColor Yellow

        $findings = @(Invoke-ScriptAnalyzer -Path $root -Recurse -Fix:$fix -ErrorAction Continue)
        if ($findings.Count -eq 0) {
            Write-Host "  PSScriptAnalyzer: clean." -ForegroundColor Green
        }
        else {
            Write-Warning ("PSScriptAnalyzer reports {0} finding(s) needing a manual fix:" -f $findings.Count)
            $findings |
                Group-Object RuleName |
                Sort-Object Count -Descending |
                ForEach-Object { Write-Warning ("  {0}: {1}" -f $_.Name, $_.Count) }
        }
    }
    else {
        Write-Host "  PSScriptAnalyzer is not installed; skipped." -ForegroundColor DarkGray
    }

    if (Get-Command markdownlint-cli2 -CommandType Application -ErrorAction SilentlyContinue) {
        Write-Host ("  markdownlint-cli2: {0} ..." -f $(if ($fix) { "fixing what is fixable" } else { "reporting only" })) -ForegroundColor Yellow

        $markdownArgs = @(if ($fix) { "--fix" }) + @("**/*.md")
        & markdownlint-cli2 @markdownArgs
        if ($LASTEXITCODE -ne 0) {
            Write-Warning ("markdownlint-cli2 reports findings needing a manual fix (exit code: {0})." -f $LASTEXITCODE)
        }
        else {
            Write-Host "  markdownlint-cli2: clean." -ForegroundColor Green
        }
    }
    else {
        Write-Host "  markdownlint-cli2 is not installed; skipped." -ForegroundColor DarkGray
    }
}

$root = Get-RepositoryRoot
Push-Location $root
try {
    Invoke-Git -Arguments "rev-parse", "--is-inside-work-tree" | Out-Null

    $branch = @(Invoke-Git -Arguments "rev-parse", "--abbrev-ref", "HEAD")[0].Trim()
    if ($branch -match $ReleaseBranchPrefix -and -not [regex]::IsMatch($branch, $ReleaseBranchPattern)) {
        throw ("'{0}' is not a release branch name. Use rel/X.Y or release/X.Y, optionally with a v and a patch part, such as release/v2.4 or rel/2.4.1." -f $branch)
    }
    $dirty = @(Invoke-Git -Arguments "status", "--porcelain" | Where-Object { $_ })
    if ($dirty.Count -gt 0) {
        throw ("The working tree has {0} uncommitted change(s). Commit or stash them first; the lint steps commit what they touch." -f $dirty.Count)
    }

    $carriers = @(Get-VersionCarrierPath -VersionFile $VersionFile -Solution $Solution)
    $versions = @($carriers | ForEach-Object { Get-ProductVersion -Path $_ })
    $distinct = @($versions | ForEach-Object { $_.Display } | Sort-Object -Unique)
    if ($distinct.Count -gt 1) {
        throw ("The version files disagree: {0}. Align them, or pass -VersionFile." -f ($distinct -join ", "))
    }
    $current = $versions[0]

    Write-Host ("Repository: {0}" -f (Get-Hyperlink -Path $root -Text $root)) -ForegroundColor Cyan
    Write-Host ("Branch:     {0}" -f $branch) -ForegroundColor Cyan
    Write-Host ("Version:    {0}" -f $current.Display) -ForegroundColor Cyan
    $carriers | ForEach-Object { Write-Host ("Version in: {0}" -f (Get-Hyperlink -Path $_)) -ForegroundColor Cyan }

    Write-Verbose ("Version file(s): {0}" -f ($carriers -join ", "))

    # Publishing a release from main means cutting a release branch, not building a dev
    # version. The release line is settled here, before the build, so a refused -Version
    # fails fast; the branch itself is cut only after build, test and lint have passed, so
    # a failing check leaves main without a stray branch or version bump. Below 1.0 nothing
    # is branched off: the run stays on main, which is bumped and tagged as a dev version.
    $cutReleaseBranch = $false
    $existingLine = $null
    if ($branch -eq "main") {
        $line = Get-ReleaseLine -Current $current -Version $Version
        if ($line.Major -gt 0) {
            $cutReleaseBranch = $true
            $relBranch = "rel/{0}.{1}" -f $line.Major, $line.Minor
            if (-not [string]::IsNullOrWhiteSpace($Version)) {
                # An explicit -Version may patch a line released straight from main (a tag but
                # no branch); Get-TargetVersion refuses a tag that is taken.
                $existingBranch = Find-ReleaseBranch -Major $line.Major -Minor $line.Minor
                if ($existingBranch) {
                    throw ("Release branch '{0}' already exists; release on it, or pass a different -Version." -f $existingBranch)
                }
            }
            else {
                # Main still carrying a released line was never advanced, possibly past several
                # released lines, so the first free line above it is settled here, before any change.
                $existingLine = Find-ReleaseLine -Major $line.Major -Minor $line.Minor
                if ($existingLine) {
                    $advanced = [pscustomobject]@{ Major = $line.Major; Minor = $line.Minor + 1 }
                    while (($taken = Find-ReleaseLine -Major $advanced.Major -Minor $advanced.Minor)) {
                        Write-Verbose ("Release line {0}.{1} is taken by {2}." -f $advanced.Major, $advanced.Minor, $taken)
                        $advanced.Minor++
                    }
                }
            }
        }
    }

    # A taken -Version is refused before the build rather than after it. With -Version main is
    # never advanced, so the branch the version is computed for is already final here.
    if (-not [string]::IsNullOrWhiteSpace($Version)) {
        $plannedBranch = if ($cutReleaseBranch) { $relBranch } else { $branch }
        Get-TargetVersion -Branch $plannedBranch -Current $current -Version $Version | Out-Null
    }

    Write-Step "1/8 Build and test"
    # With a solution: build it, then Test-Sln reuses that build (--no-build). Without
    # one: skip the build and run the default project runner directly. See
    # Resolve-SolutionPath in Common.ps1.
    $slnPath = Resolve-SolutionPath -Path $Solution

    if ($slnPath) {
        $solutionArgs = @("-Solution", $slnPath)
        Write-Host ("  Solution: {0}" -f (Get-Hyperlink -Path $slnPath)) -ForegroundColor DarkGray
        Invoke-Step -Script "Build-Sln.ps1" -Arguments (@("-Configuration", $Configuration) + $solutionArgs) -FailMessage "build failed"
        Invoke-Step -Script "Test-Sln.ps1" -Arguments (@("-Configuration", $Configuration) + $solutionArgs + @("--no-build")) -FailMessage "tests failed"
        Write-Host "  Build and tests passed." -ForegroundColor Green
    }
    else {
        # Say so explicitly: a silent step here reads as "built fine" when nothing was built.
        Write-Host "  No solution found; running the test projects directly." -ForegroundColor DarkGray
        Invoke-Step -Script "Test-Csprojs.ps1" -Arguments @("-Configuration", $Configuration) -FailMessage "tests failed"
        Write-Host "  Tests passed." -ForegroundColor Green
    }

    Write-Step "2/8 Code lint"
    $lintSolutionArgs = if (-not [string]::IsNullOrWhiteSpace($Solution)) { @("-Solution", $Solution) } else { @() }
    if ($WhatIfPreference) {
        # -Verify prints nothing when it is happy, and a silent step reads as a skipped one.
        Invoke-Step -Script "Lint-Code.ps1" -Arguments (@("-Verify") + $lintSolutionArgs) -FailMessage "formatting violations found"
        Write-Host "  Formatting is clean." -ForegroundColor Green
    }
    else {
        Invoke-Step -Script "Lint-Code.ps1" -Arguments $lintSolutionArgs -FailMessage "formatting failed"
        Save-WorkingTreeChange -Message "style: apply code formatting" | Out-Null
    }

    Write-Step "3/8 Other lints"
    Invoke-OtherLint
    Save-WorkingTreeChange -Message "style: apply lint fixes" | Out-Null

    # rel/X.Y is opened at the checked commit, so any lint fixes land on main as well. When
    # that line already exists main still holds a version that has been taken, so main is
    # advanced to the first free line (kept -dev) and that line is cut instead. Each git-state
    # change reuses an existing helper (COD-10): Set-ProductVersion advances main, Commit-Version
    # commits that bump. The bump is pushed with everything else in step 8, so an unreachable remote
    # cannot stop the run halfway.
    $createdReleaseBranch = $false
    if ($branch -eq "main") {
        Write-Step "Release branch"
    }

    if ($branch -eq "main" -and -not $cutReleaseBranch) {
        Write-Host ("  {0}.{1} is below 1.0: no release branch; main is bumped and tagged as a dev version." -f $line.Major, $line.Minor) -ForegroundColor Yellow
    }
    elseif ($cutReleaseBranch) {
        if ($existingLine) {
            $prefix = [version] ("{0}.{1}.0" -f $advanced.Major, $advanced.Minor)
            Write-Host ("  {0} exists, but main still carries {1} on the released line {2}.{3}. Advancing main to {4}-dev." -f $existingLine, $current.Display, $line.Major, $line.Minor, $prefix) -ForegroundColor Yellow
            foreach ($carrier in $carriers) {
                Set-ProductVersion -Path $carrier -Prefix $prefix -Suffix dev -Confirm:$false
            }
            foreach ($carrier in @($carriers | Select-Object -Skip 1)) {
                if ($PSCmdlet.ShouldProcess($carrier, "git add")) { Invoke-Git -Arguments "add", "--", $carrier | Out-Null }
            }
            & (Join-Path $PSScriptRoot "Commit-Version.ps1") -VersionFile $carriers[0] -Prefix $prefix -Suffix dev -NoPush

            # Under -WhatIf the file was left alone, so re-reading it would return the taken version.
            $current = if ($WhatIfPreference) {
                [pscustomobject]@{
                    Path    = $current.Path
                    Prefix  = $prefix
                    Suffix  = "dev"
                    Style   = $current.Style
                    Display = "$prefix-dev"
                }
            }
            else {
                Get-ProductVersion -Path $carriers[0]
            }
            $relBranch = "rel/{0}.{1}" -f $advanced.Major, $advanced.Minor
        }

        if ($PSCmdlet.ShouldProcess($relBranch, "git checkout -b at the current commit")) {
            Invoke-Git -Arguments "checkout", "-b", $relBranch | Out-Null
            $createdReleaseBranch = $true
        }
        Write-Host ("  Release branch: {0} (cut from main at the current commit)." -f $relBranch) -ForegroundColor Green
        $branch = $relBranch
    }

    Write-Step "4/8 Version"
    $target = Get-TargetVersion -Branch $branch -Current $current -Version $Version
    Write-Host ("  {0} -> {1}" -f $current.Display, $target.Display) -ForegroundColor Yellow
    $isCleanRelease = -not $target.Suffix -and [regex]::IsMatch($branch, $ReleaseBranchPattern)
    if ($isCleanRelease) {
        # A clean, unsuffixed version publishes a real release; never let that pass unannounced.
        Write-Host "  Release branch: publishing a clean, unsuffixed release." -ForegroundColor DarkGray
    }
    foreach ($carrier in $carriers) {
        Set-ProductVersion -Path $carrier -Prefix $target.Prefix -Suffix $target.Suffix -Confirm:$false
    }

    Write-Step "5/8 Commit version"
    # Under -WhatIf the file was left alone, so git sees no change; the version decides instead.
    $versionChanges = if ($WhatIfPreference) {
        @($target.Display | Where-Object { $_ -ne $current.Display })
    }
    else {
        @(Invoke-Git -Arguments (@("status", "--porcelain", "--") + $carriers) | Where-Object { $_ })
    }
    if ($versionChanges.Count -eq 0) {
        # The file already carried the target version, so there is nothing to commit
        # and the tag belongs on the commit that is already there.
        Write-Host "  The version file is already up to date; nothing to commit." -ForegroundColor DarkGray
    }
    else {
        # Commit-Version.ps1 stages one file and commits the index, so the remaining
        # carriers only have to be staged beforehand.
        foreach ($carrier in @($carriers | Select-Object -Skip 1)) {
            if ($PSCmdlet.ShouldProcess($carrier, "git add")) {
                Invoke-Git -Arguments "add", "--", $carrier | Out-Null
            }
        }
        & (Join-Path $PSScriptRoot "Commit-Version.ps1") -VersionFile $carriers[0] -Prefix $target.Prefix -Suffix $target.Suffix -NoPush
    }

    Write-Step "6/8 Tag commit"
    & (Join-Path $PSScriptRoot "Tag-Commit.ps1") -VersionFile $carriers[0] -Prefix $target.Prefix -Suffix $target.Suffix -NoPush

    Write-Step "7/8 Merge into main"
    $mergeStatus = "Skipped"
    if ($NoMerge) {
        Write-Host "  -NoMerge: main is left alone; merge the release into it by hand." -ForegroundColor Yellow
    }
    elseif (-not $isCleanRelease) {
        Write-Host "  Not a clean release on a release branch; nothing to merge." -ForegroundColor DarkGray
    }
    else {
        $mergeStatus = Merge-ReleaseIntoMain -Branch $branch -Release $target -Carriers $carriers
    }

    Write-Step "8/8 Push"
    $pushes = [System.Collections.Generic.List[string[]]]::new()
    # A release cut from main also pushes main: it may carry the version advance. So does a
    # release merged into main.
    if ($createdReleaseBranch -or $mergeStatus -eq "Merged") { $pushes.Add(@("origin", "main")) }
    # --set-upstream always: a release branch cut on a run whose push failed has no upstream yet.
    $pushes.Add(@("--set-upstream", "origin", $branch))
    $pushes.Add(@("origin", "--tags"))

    $pending = [System.Collections.Generic.List[string[]]]::new()
    $pushFailed = $false
    if ($NoPush) {
        Write-Host ("  -NoPush: the branch and the tag {0} stay local." -f $target.Tag) -ForegroundColor Yellow
        $pending.AddRange($pushes)
    }
    elseif ($PSCmdlet.ShouldProcess(("{0} and tag {1}" -f $branch, $target.Tag), "git push")) {
        # Each push is tried even when an earlier one failed; the failed ones are retried by hand.
        foreach ($push in $pushes) {
            if (-not (Invoke-GitPush -Arguments $push)) { $pending.Add($push) }
        }
        $pushFailed = $pending.Count -gt 0
        if (-not $pushFailed) { Write-Host "  Pushed." -ForegroundColor Green }
    }

    Write-Host ""
    if ($pending.Count -eq 0) {
        Write-Host ("Released {0} on {1} as tag {2}." -f $target.Display, $branch, $target.Tag) -ForegroundColor Green
    }
    else {
        Write-Host ("Released {0} on {1} as tag {2} locally. To publish it, run:" -f $target.Display, $branch, $target.Tag) -ForegroundColor Yellow
        $pending | ForEach-Object { Write-Host ("  git push {0}" -f ($_ -join " ")) -ForegroundColor Yellow }
    }
}
finally {
    Pop-Location
}

if ($pushFailed -or $mergeStatus -eq "Failed") { exit (Get-AiKitExitCode Git) }
