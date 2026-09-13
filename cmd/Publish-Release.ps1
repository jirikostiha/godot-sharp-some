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
  7. The branch and the tags are pushed.

Version rules by branch:

  -Version            Sets the numeric part outright; the suffix still follows the
                      rules below.
  rel/* or release/*  The suffix is empty (a clean release). The patch part is
                      raised until the version outranks every version tag reachable
                      from HEAD, so a fresh release branch keeps its prefix and a
                      re-run moves to the next patch.
  anything else       The suffix is dev and the minor part is raised until the
                      version is above the last release tag and above every
                      version tag reachable from HEAD.

On main the run does not build a dev version: it cuts a release. rel/X.Y is opened at
the current commit, with X.Y taken from -Version or, without it, from the minor main
carries, and the release then proceeds on that branch. When rel/X.Y already exists main
still holds a version that has been taken, so main is first advanced one minor with -dev
(reusing Bump-Version and Commit-Version) and rel/(X.Y+1) is cut instead.

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
rel/4.2 already exists, advances main to 4.3.0-dev, cuts rel/4.3 and releases 4.3.0.

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
description: Runs the release pipeline end to end - build, test, lint, version bump by branch type, commit, tag and push. Use to cut a release.
profiles: [dotnet]
version: 3.1.0
---#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string] $Solution,
    [string] $VersionFile = (Join-Path $PSScriptRoot ".." "product_version.props"),
    [ValidatePattern('(?i)^v?\d+(\.\d+){1,3}$')]
    [string] $Version,
    [string] $Configuration = "Release",
    [switch] $NoPush
)

. (Join-Path $PSScriptRoot "Common.ps1")

$ErrorActionPreference = "Stop"
# Native exit codes are checked explicitly below; letting the preference throw on
# any stderr write would turn ordinary git progress output into a failure.
$PSNativeCommandUseErrorActionPreference = $false

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

function ConvertTo-ThreePartVersion {
    <#
    .SYNOPSIS
    Normalises a version to exactly three parts.

    .DESCRIPTION
    A tag such as v2.4 parses to a version whose build part is -1, which does not
    compare usefully against 2.4.0. Missing parts become zero.

    .PARAMETER Version
    The version to normalise.

    .EXAMPLE
    ConvertTo-ThreePartVersion -Version ([version]"2.4")
    #>
    [CmdletBinding()]
    [OutputType([version])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [version] $Version
    )

    return [version] ("{0}.{1}.{2}" -f $Version.Major, $Version.Minor, [Math]::Max($Version.Build, 0))
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

    if ($Branch -match '^(rel|release)/') {
        # A release branch names the version it prepares; honour that name when the
        # file still carries the lower version inherited from main. An explicit version
        # outranks the branch name - it was typed for this run.
        $named = [regex]::Match($Branch, '^(?:rel|release)/v?(\d+(?:\.\d+){1,3})')
        if (-not $isExplicit -and $named.Success) {
            $branchVersion = ConvertTo-ThreePartVersion ([version] $named.Groups[1].Value)
            if ($branchVersion -gt $prefix) { $prefix = $branchVersion }
        }

        # A release branch always ships a clean, unsuffixed release.
        $suffix = ""

        $nextPrefix = { param($p) [version] ("{0}.{1}.{2}" -f $p.Major, $p.Minor, ($p.Build + 1)) }
    }
    else {
        $suffix = "dev"

        # main stays above the last release, which usually lives on a release
        # branch and is therefore not reachable from here.
        $lastRelease = @($allTags | Where-Object { $_.Rank -eq 2 } | Sort-Object Prefix | Select-Object -Last 1)
        if (-not $isExplicit -and $lastRelease.Count -eq 1) {
            $floor = [version] ("{0}.{1}.0" -f $lastRelease[0].Prefix.Major, ($lastRelease[0].Prefix.Minor + 1))
            if ($floor -gt $prefix) { $prefix = $floor }
        }

        $nextPrefix = { param($p) [version] ("{0}.{1}.0" -f $p.Major, ($p.Minor + 1)) }
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

function Test-BranchExists {
    <#
    .SYNOPSIS
    Determines whether a branch exists locally or on the origin.

    .DESCRIPTION
    Checks the local ref, then the remote-tracking ref, then the remote itself for a
    branch pushed elsewhere but not yet fetched. An unreachable remote is tolerated:
    the local checks still stand.

    .PARAMETER Name
    The branch name, for example rel/4.2.

    .EXAMPLE
    Test-BranchExists -Name "rel/4.2"
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $Name
    )

    if (@(Invoke-Git -Arguments "branch", "--list", $Name | Where-Object { $_ }).Count -gt 0) { return $true }
    if (@(Invoke-Git -Arguments "branch", "--remotes", "--list", ("origin/{0}" -f $Name) | Where-Object { $_ }).Count -gt 0) { return $true }

    $remote = @(Invoke-Git -Arguments "ls-remote", "--heads", "origin", $Name -AllowFailure | Where-Object { $_ })
    return $remote.Count -gt 0
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
    # version: rel/X.Y is opened at the current commit for the version main carries. When
    # that branch already exists main still holds a version that has been taken, so main is
    # advanced one minor (kept -dev) and the next line is cut instead. A release branch and
    # any other branch fall through unchanged (clean release / dev build). Each git-state
    # change reuses an existing script (COD-10): Bump-Version advances main, Commit-Version
    # commits that bump.
    $createdReleaseBranch = $false
    if ($branch -eq "main") {
        Write-Step "Release branch"
        $line = Get-ReleaseLine -Current $current -Version $Version
        $relBranch = "rel/{0}.{1}" -f $line.Major, $line.Minor

        if (Test-BranchExists -Name $relBranch) {
            if (-not [string]::IsNullOrWhiteSpace($Version)) {
                throw ("Release branch '{0}' already exists; release on it, or pass a different -Version." -f $relBranch)
            }

            Write-Host ("  {0} exists; main still carries that version. Advancing main one minor." -f $relBranch) -ForegroundColor Yellow
            foreach ($carrier in $carriers) {
                & (Join-Path $PSScriptRoot "Bump-Version.ps1") -VersionFile $carrier -Minor -Suffix dev -Confirm:$false
            }
            foreach ($carrier in @($carriers | Select-Object -Skip 1)) {
                if ($PSCmdlet.ShouldProcess($carrier, "git add")) { Invoke-Git -Arguments "add", "--", $carrier | Out-Null }
            }
            & (Join-Path $PSScriptRoot "Commit-Version.ps1") -VersionFile $carriers[0] -NoPush:$NoPush

            $current = Get-ProductVersion -Path $carriers[0]
            $advanced = ConvertTo-ThreePartVersion $current.Prefix
            $relBranch = "rel/{0}.{1}" -f $advanced.Major, $advanced.Minor
            if (Test-BranchExists -Name $relBranch) {
                throw ("The advanced release branch '{0}' also exists; resolve the release branches manually." -f $relBranch)
            }
        }

        if ($PSCmdlet.ShouldProcess($relBranch, "git checkout -b at the current commit")) {
            Invoke-Git -Arguments "checkout", "-b", $relBranch | Out-Null
            $createdReleaseBranch = $true
        }
        Write-Host ("  Release branch: {0} (cut from main at the current commit)." -f $relBranch) -ForegroundColor Green
        $branch = $relBranch
    }

    Write-Step "1/7 Build and test"
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

    Write-Step "2/7 Code lint"
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

    Write-Step "3/7 Other lints"
    Invoke-OtherLint
    Save-WorkingTreeChange -Message "style: apply lint fixes" | Out-Null

    Write-Step "4/7 Version"
    $target = Get-TargetVersion -Branch $branch -Current $current -Version $Version
    Write-Host ("  {0} -> {1}" -f $current.Display, $target.Display) -ForegroundColor Yellow
    if (-not $target.Suffix -and $branch -match '^(rel|release)/') {
        # A clean, unsuffixed version publishes a real release; never let that pass unannounced.
        Write-Host "  Release branch: publishing a clean, unsuffixed release." -ForegroundColor DarkGray
    }
    foreach ($carrier in $carriers) {
        Set-ProductVersion -Path $carrier -Prefix $target.Prefix -Suffix $target.Suffix -Confirm:$false
    }

    Write-Step "5/7 Commit version"
    $versionChanges = @(Invoke-Git -Arguments (@("status", "--porcelain", "--") + $carriers) | Where-Object { $_ })
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
        & (Join-Path $PSScriptRoot "Commit-Version.ps1") -VersionFile $carriers[0] -NoPush
    }

    Write-Step "6/7 Tag commit"
    & (Join-Path $PSScriptRoot "Tag-Commit.ps1") -VersionFile $carriers[0] -NoPush

    Write-Step "7/7 Push"
    if ($NoPush) {
        Write-Host ("  -NoPush: the branch and the tag {0} stay local." -f $target.Tag) -ForegroundColor Yellow
    }
    elseif ($PSCmdlet.ShouldProcess(("{0} and tag {1}" -f $branch, $target.Tag), "git push")) {
        # A freshly cut release branch has no upstream yet; set one on the first push.
        if ($createdReleaseBranch) {
            Invoke-Git -Arguments "push", "--set-upstream", "origin", $branch | Out-Null
        }
        else {
            Invoke-Git -Arguments "push" | Out-Null
        }
        Invoke-Git -Arguments "push", "origin", "--tags" | Out-Null
        Write-Host "  Pushed." -ForegroundColor Green
    }

    Write-Host ""
    Write-Host ("Released {0} on {1} as tag {2}." -f $target.Display, $branch, $target.Tag) -ForegroundColor Green
}
finally {
    Pop-Location
}
