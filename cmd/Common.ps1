<#
.SYNOPSIS
Shared helper functions for the scripts in the cmd folder.

.DESCRIPTION
Dot-source this file from any script in the cmd folder to gain access to the
common helpers:

    . (Join-Path $PSScriptRoot "Common.ps1")

Provided helpers:
- Get-Hyperlink        Renders a clickable terminal hyperlink (OSC 8).
- Get-RepositoryRoot   Resolves the repository root folder.
- Get-RepositoryConfig Reads the optional "repo" block from .aikit.json.
- Get-SolutionPath     Resolves the full path of the main solution file.
- Get-VersionFile      Resolves the file that carries the product version.
- Get-ProductVersion   Reads the version prefix and suffix from that file.
- Set-ProductVersion   Writes the version prefix and suffix back.

A repository may add its own helpers in Common.local.ps1 beside this file; it is
dot-sourced at the end if present. Put repository-specific functions there, not
here: this file comes from the ai-kit and is overwritten on every sync.

.EXAMPLE
. (Join-Path $PSScriptRoot "Common.ps1")
$slnPath = Get-SolutionPath
Write-Host ("Building {0}" -f (Get-Hyperlink -Path $slnPath))
#>
<#---
name: Common
kind: cmd
description: Shared helpers dot-sourced by the other scripts in cmd.
version: 2.4.0
---#>

# Note: this file is dot-sourced, so it runs in the caller's scope. It therefore
# deliberately does not call Set-StrictMode or change any other preference: doing
# so would silently alter the behaviour of every script that dot-sources it.

function Test-HyperlinkSupport {
    <#
    .SYNOPSIS
    Determines whether the current host can render OSC 8 terminal hyperlinks.

    .DESCRIPTION
    Returns $false when output is redirected (file, pipe, CI log) or when the
    environment opts out of decorated output via NO_COLOR or TERM=dumb.
    In those cases callers should fall back to plain text.

    .EXAMPLE
    if (Test-HyperlinkSupport) { "terminal supports hyperlinks" }
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param()

    if (-not [string]::IsNullOrEmpty($env:NO_COLOR)) { return $false }
    if ($env:TERM -eq "dumb") { return $false }
    if ([Console]::IsOutputRedirected) { return $false }

    return $true
}

function ConvertTo-HyperlinkUri {
    <#
    .SYNOPSIS
    Converts a file system path into an absolute URI usable in a hyperlink.

    .DESCRIPTION
    Absolute and relative file system paths are converted to file:// URIs.
    Values that already are absolute URIs (for example https://) are returned
    unchanged. The path does not need to exist.

    .PARAMETER Path
    The file system path or absolute URI to convert.

    .EXAMPLE
    ConvertTo-HyperlinkUri -Path ".\src\app.slnx"
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $Path
    )

    $uri = $null
    if ([Uri]::TryCreate($Path, [UriKind]::Absolute, [ref] $uri) -and -not $uri.IsFile) {
        return $uri.AbsoluteUri
    }

    try {
        # Resolves against the current PowerShell location and, unlike Resolve-Path,
        # does not require the path to exist.
        $fullPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Path)
        return ([Uri] $fullPath).AbsoluteUri
    }
    catch {
        Write-Verbose ("Cannot convert path to URI: '{0}'. {1}" -f $Path, $_.Exception.Message)
        return $Path
    }
}

function Get-Hyperlink {
    <#
    .SYNOPSIS
    Builds a clickable terminal hyperlink for a path.

    .DESCRIPTION
    Wraps the display text in an OSC 8 escape sequence so supporting terminals
    (Windows Terminal, VS Code, most modern terminals) render it as a clickable
    link to the target path. When the host cannot render hyperlinks — for
    example when output is redirected — the plain display text is returned, so
    the result is always safe to write to any stream.

    .PARAMETER Path
    The link target. A file system path (absolute or relative) or an absolute
    URI. Accepts pipeline input, including FileInfo/DirectoryInfo objects via
    their FullName property.

    .PARAMETER Text
    The text shown to the user. Defaults to the leaf name of Path.

    .EXAMPLE
    Get-Hyperlink -Path "C:\repo\src\app.slnx"
    Returns a clickable link labelled "app.slnx".

    .EXAMPLE
    Get-Item .\Common.ps1 | Get-Hyperlink
    Pipes a file into the helper and links it using its leaf name as the label.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [ValidateNotNullOrEmpty()]
        [Alias("FullName")]
        [string] $Path,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName)]
        [string] $Text
    )

    process {
        # The leaf name is the useful label at almost every call site; the full path
        # was the old default and every caller overrode it.
        $label = if ([string]::IsNullOrWhiteSpace($Text)) { Split-Path -Leaf $Path } else { $Text }

        if (-not (Test-HyperlinkSupport)) {
            return $label
        }

        $uri = ConvertTo-HyperlinkUri -Path $Path
        $esc = [char] 0x1B

        return "$esc]8;;$uri$esc\$label$esc]8;;$esc\"
    }
}

function Get-RepositoryRoot {
    <#
    .SYNOPSIS
    Gets the full path of the repository root.

    .DESCRIPTION
    The repository root is the parent folder of the cmd folder that contains
    this file, so the result is independent of the current working directory.

    .EXAMPLE
    Get-RepositoryRoot
    Returns for example C:\Repositories\my-repo.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param()

    return (Get-Item -LiteralPath (Join-Path $PSScriptRoot "..") -ErrorAction Stop).FullName
}

function Get-RepositoryConfig {
    <#
    .SYNOPSIS
    Reads the optional "repo" block from .aikit.json in the repository root.

    .DESCRIPTION
    Discovery covers nearly every repository, so this block exists only for the
    ones where it cannot: several solutions, or a version held somewhere unusual.
    A missing or unreadable file yields an empty result rather than an error,
    because every caller has a working fallback.

    Recognised keys:
      solution     Path to the main solution file, relative to the repository root.
      versionFile  Path to the file carrying the product version, likewise relative.

    .EXAMPLE
    (Get-RepositoryConfig).solution
    #>
    [CmdletBinding()]
    [OutputType([hashtable])]
    param()

    $empty = @{}
    $configPath = Join-Path (Get-RepositoryRoot) ".aikit.json"
    if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) { return $empty }

    try {
        $json = Get-Content -LiteralPath $configPath -Raw -ErrorAction Stop
        if ([string]::IsNullOrWhiteSpace($json)) { return $empty }

        $parsed = $json | ConvertFrom-Json -ErrorAction Stop
        if ($parsed.PSObject.Properties.Name -notcontains "repo") { return $empty }

        $result = @{}
        foreach ($property in $parsed.repo.PSObject.Properties) {
            $result[$property.Name] = $property.Value
        }
        return $result
    }
    catch {
        Write-Verbose ("Cannot read '{0}': {1}" -f $configPath, $_.Exception.Message)
        return $empty
    }
}

function Get-RepositoryFile {
    <#
    .SYNOPSIS
    Enumerates repository files with the given extensions, skipping generated folders.

    .DESCRIPTION
    Shared by solution and version discovery. The exclusions matter more than they
    look: .vs holds a copy of the solution under its own name, and bin and obj hold
    copies of project files, so searching without them finds the wrong file first.

    .PARAMETER Extension
    The extensions to accept, including the leading dot.

    .EXAMPLE
    Get-RepositoryFile -Extension ".slnx", ".sln"
    #>
    [CmdletBinding()]
    [OutputType([System.IO.FileInfo])]
    param(
        [Parameter(Mandatory)]
        [string[]] $Extension
    )

    $root = Get-RepositoryRoot
    $excluded = @(".vs", ".git", ".ai", "bin", "obj", "artf", "node_modules", "worktrees", "packages")

    Get-ChildItem -LiteralPath $root -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.Extension -in $Extension } |
        Where-Object {
            $relative = [IO.Path]::GetRelativePath($root, $_.DirectoryName)
            $segments = $relative -split ([regex]::Escape([IO.Path]::DirectorySeparatorChar) + "|/")
            -not ($segments | Where-Object { $_ -in $excluded })
        }
}

function Get-SolutionPath {
    <#
    .SYNOPSIS
    Gets the full path of the main solution file.

    .DESCRIPTION
    Uses repo.solution from .aikit.json when set. Otherwise searches the
    repository, preferring .slnx over .sln and a shallower path over a deeper one,
    so a solution in the root or in src wins over one belonging to a sample.

    Throws when nothing is found, and when several candidates rank equally —
    guessing there would build the wrong thing silently.

    .PARAMETER Path
    An explicit solution path, absolute or relative to the repository root.
    Overrides both configuration and discovery.

    .EXAMPLE
    dotnet build (Get-SolutionPath)
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [string] $Path
    )

    $root = Get-RepositoryRoot

    if ([string]::IsNullOrWhiteSpace($Path)) {
        $Path = (Get-RepositoryConfig).solution
    }

    if (-not [string]::IsNullOrWhiteSpace($Path)) {
        $configured = if ([IO.Path]::IsPathRooted($Path)) { $Path } else { Join-Path $root $Path }
        if (-not (Test-Path -LiteralPath $configured -PathType Leaf)) {
            throw ("Solution file not found: '{0}'." -f $configured)
        }
        return (Get-Item -LiteralPath $configured -ErrorAction Stop).FullName
    }

    $candidates = @(Get-RepositoryFile -Extension ".slnx", ".sln")

    if ($candidates.Count -eq 0) {
        throw ("No solution file found under '{0}'. Set repo.solution in .aikit.json." -f $root)
    }

    $depth = { ([IO.Path]::GetRelativePath($root, $args[0].FullName) -split "/|\\").Count }

    $ranked = @(
        $candidates | Sort-Object `
            @{ Expression = { if ($_.Extension -eq ".slnx") { 0 } else { 1 } } },
            @{ Expression = { & $depth $_ } },
            @{ Expression = { $_.Name } }
    )

    $best = $ranked[0]
    $tied = @($ranked | Where-Object {
        $_.Extension -eq $best.Extension -and (& $depth $_) -eq (& $depth $best)
    })

    if ($tied.Count -gt 1) {
        $list = ($tied | ForEach-Object { [IO.Path]::GetRelativePath($root, $_.FullName) }) -join ", "
        throw ("Several solution files rank equally: {0}. Set repo.solution in .aikit.json." -f $list)
    }

    return $best.FullName
}

function Test-VersionCarrier {
    <#
    .SYNOPSIS
    Determines whether an MSBuild file declares a product version.

    .PARAMETER Path
    The file to inspect. A file that is not valid XML is treated as "no".

    .EXAMPLE
    Test-VersionCarrier -Path .\src\App\App.csproj
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $Path
    )

    try {
        $xml = [xml](Get-Content -LiteralPath $Path -Raw -ErrorAction Stop)
    }
    catch {
        Write-Verbose ("Not valid XML, skipped: '{0}'." -f $Path)
        return $false
    }

    if ($null -eq $xml.Project) { return $false }

    foreach ($group in @($xml.Project.PropertyGroup)) {
        if ($null -eq $group) { continue }
        foreach ($name in @("VersionPrefix", "Version")) {
            $node = $group.SelectSingleNode($name)
            if ($null -ne $node -and -not [string]::IsNullOrWhiteSpace($node.InnerText)) { return $true }
        }
    }

    return $false
}

function Get-VersionFile {
    <#
    .SYNOPSIS
    Gets the full path of the file that carries the product version.

    .DESCRIPTION
    Two conventions are in use, and both are supported:

      - a dedicated product_version.props in the repository root;
      - the version held directly in a project or props file, as VersionPrefix
        with an optional VersionSuffix, or as a single Version element.

    Resolution order: repo.versionFile from .aikit.json, then product_version.props,
    then the only file that declares a version — with Directory.Build.props winning
    over individual project files, since it applies to all of them.

    .PARAMETER Path
    An explicit path, absolute or relative to the repository root. Overrides
    both configuration and discovery.

    .EXAMPLE
    Get-VersionFile
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [string] $Path
    )

    $root = Get-RepositoryRoot

    if ([string]::IsNullOrWhiteSpace($Path)) {
        $Path = (Get-RepositoryConfig).versionFile
    }

    if (-not [string]::IsNullOrWhiteSpace($Path)) {
        $configured = if ([IO.Path]::IsPathRooted($Path)) { $Path } else { Join-Path $root $Path }
        if (-not (Test-Path -LiteralPath $configured -PathType Leaf)) {
            throw ("Version file not found: '{0}'." -f $configured)
        }
        return (Get-Item -LiteralPath $configured -ErrorAction Stop).FullName
    }

    $conventional = Join-Path $root "product_version.props"
    if (Test-Path -LiteralPath $conventional -PathType Leaf) {
        return (Get-Item -LiteralPath $conventional).FullName
    }

    $searched = @(Get-RepositoryFile -Extension ".props", ".csproj", ".fsproj", ".vbproj")
    $carriers = @($searched | Where-Object { Test-VersionCarrier -Path $_.FullName })

    if ($carriers.Count -eq 0) {
        throw ("No file carrying a product version found under '{0}'. Set repo.versionFile in .aikit.json." -f $root)
    }

    $shared = @($carriers | Where-Object { $_.Name -eq "Directory.Build.props" })
    if ($shared.Count -eq 1) { return $shared[0].FullName }
    if ($carriers.Count -eq 1) { return $carriers[0].FullName }

    $list = ($carriers | ForEach-Object { [IO.Path]::GetRelativePath($root, $_.FullName) }) -join ", "
    throw ("Several files carry a product version: {0}. Set repo.versionFile in .aikit.json." -f $list)
}

function Get-ProductVersion {
    <#
    .SYNOPSIS
    Reads the product version from the file that carries it.

    .DESCRIPTION
    Returns the prefix, the suffix, the path they came from and the layout in use.
    Both layouts are understood: VersionPrefix with an optional VersionSuffix, and
    a single Version element such as 1.4.2-rc, which is split on the first hyphen.

    .PARAMETER Path
    The version file to read. Resolved by Get-VersionFile when omitted.

    .EXAMPLE
    (Get-ProductVersion).Display
    #>
    [CmdletBinding()]
    [OutputType([pscustomobject])]
    param(
        [string] $Path
    )

    $file = Get-VersionFile -Path $Path
    $xml = [xml](Get-Content -LiteralPath $file -Raw -ErrorAction Stop)

    $prefixNode = $null
    $suffixNode = $null
    $singleNode = $null

    foreach ($group in @($xml.Project.PropertyGroup)) {
        if ($null -eq $group) { continue }
        if ($null -eq $prefixNode) { $prefixNode = $group.SelectSingleNode("VersionPrefix") }
        if ($null -eq $suffixNode) { $suffixNode = $group.SelectSingleNode("VersionSuffix") }
        if ($null -eq $singleNode) { $singleNode = $group.SelectSingleNode("Version") }
    }

    $prefixText = ""
    $suffixText = ""
    $style = "prefix"

    if ($null -ne $prefixNode -and -not [string]::IsNullOrWhiteSpace($prefixNode.InnerText)) {
        $prefixText = $prefixNode.InnerText.Trim()
        if ($null -ne $suffixNode) { $suffixText = $suffixNode.InnerText.Trim() }
    }
    elseif ($null -ne $singleNode -and -not [string]::IsNullOrWhiteSpace($singleNode.InnerText)) {
        $style = "single"
        $combined = $singleNode.InnerText.Trim()
        $split = $combined.IndexOf("-")
        if ($split -ge 0) {
            $prefixText = $combined.Substring(0, $split)
            $suffixText = $combined.Substring($split + 1)
        }
        else {
            $prefixText = $combined
        }
    }
    else {
        throw ("No VersionPrefix or Version element found in '{0}'." -f $file)
    }

    try {
        $prefix = [version] $prefixText
    }
    catch {
        throw ("Invalid version format in '{0}': '{1}'." -f $file, $prefixText)
    }

    [pscustomobject]@{
        Path    = $file
        Prefix  = $prefix
        Suffix  = $suffixText
        Style   = $style
        Display = if ($suffixText) { "$prefix-$suffixText" } else { "$prefix" }
    }
}

function Set-ProductVersion {
    <#
    .SYNOPSIS
    Writes the product version back to the file that carries it.

    .DESCRIPTION
    Writes in whichever layout the file already uses, so a repository holding a
    single Version element does not silently gain a VersionPrefix pair. A missing
    VersionSuffix element is created only when there is a suffix to put in it.

    .PARAMETER Path
    The version file to write. Resolved by Get-VersionFile when omitted.

    .PARAMETER Prefix
    The version prefix to write, for example 1.5.0.

    .PARAMETER Suffix
    The version suffix to write, for example rc. An empty string clears it.

    .EXAMPLE
    Set-ProductVersion -Prefix ([version]"1.5.0") -Suffix "rc"
    #>
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [string] $Path,

        [Parameter(Mandatory)]
        [version] $Prefix,

        [string] $Suffix = ""
    )

    $current = Get-ProductVersion -Path $Path
    $file = $current.Path
    $xml = [xml](Get-Content -LiteralPath $file -Raw -ErrorAction Stop)

    if (-not $PSCmdlet.ShouldProcess($file, ("Set version to {0}" -f $Prefix))) { return }

    if ($current.Style -eq "single") {
        $node = $null
        foreach ($group in @($xml.Project.PropertyGroup)) {
            if ($null -eq $group) { continue }
            if ($null -eq $node) { $node = $group.SelectSingleNode("Version") }
        }
        $node.InnerText = if ($Suffix) { "$Prefix-$Suffix" } else { "$Prefix" }
    }
    else {
        $prefixNode = $null
        $suffixNode = $null
        $owner = $null
        foreach ($group in @($xml.Project.PropertyGroup)) {
            if ($null -eq $group) { continue }
            if ($null -eq $prefixNode) {
                $prefixNode = $group.SelectSingleNode("VersionPrefix")
                if ($null -ne $prefixNode) { $owner = $group }
            }
            if ($null -eq $suffixNode) { $suffixNode = $group.SelectSingleNode("VersionSuffix") }
        }

        $prefixNode.InnerText = "$Prefix"

        if ($null -eq $suffixNode) {
            if ($Suffix) {
                $suffixNode = $xml.CreateElement("VersionSuffix")
                $owner.AppendChild($suffixNode) | Out-Null
                $suffixNode.InnerText = $Suffix
            }
        }
        else {
            $suffixNode.InnerText = $Suffix
        }
    }

    $xml.Save($file)
}

function Resolve-SolutionPath {
    <#
    .SYNOPSIS
    Resolves the solution path, or returns nothing when the repository has none.

    .DESCRIPTION
    A thin wrapper over Get-SolutionPath with a softer contract for callers that
    can proceed without a solution: an explicit -Path that cannot be found is still
    an error, but discovery finding nothing returns $null instead of throwing, so
    the caller can fall back (for example to Test-Csprojs.ps1).

    .PARAMETER Path
    An explicit solution path, absolute or relative to the repository root.

    .EXAMPLE
    $sln = Resolve-SolutionPath -Path $Solution
    if (-not $sln) { "no solution; using the project runner" }
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [string] $Path
    )

    if (-not [string]::IsNullOrWhiteSpace($Path)) {
        return Get-SolutionPath -Path $Path
    }

    try { return Get-SolutionPath } catch { return $null }
}

function Get-AiKitExitCode {
    <#
    .SYNOPSIS
    Maps a standardized outcome name to its exit code.

    .DESCRIPTION
    One code table shared by every cmd script, so a caller (a human, CI, or an AI agent) can
    branch on the number alone without parsing console text:

        0 Success       6 Lint         formatting or analyzer violations
        1 Failure       7 Git          a git operation failed
        2 Usage         bad parameters or invalid input
        3 Precondition  environment not ready (no solution, dirty tree, missing file)
        4 Build         compilation failed
        5 Test          tests failed

    .PARAMETER Name
    The outcome name.

    .EXAMPLE
    exit (Get-AiKitExitCode Build)
    #>
    [CmdletBinding()]
    [OutputType([int])]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateSet("Success", "Failure", "Usage", "Precondition", "Build", "Test", "Lint", "Git")]
        [string] $Name
    )

    return @{
        Success = 0; Failure = 1; Usage = 2; Precondition = 3
        Build = 4; Test = 5; Lint = 6; Git = 7
    }[$Name]
}

function Stop-Script {
    <#
    .SYNOPSIS
    Ends the current script with a standardized exit code and one terse error line.

    .DESCRIPTION
    Writes the message (if any) as a single line to stderr and exits with the code for Reason
    (see Get-AiKitExitCode). Keeps success output empty and failure output to one line, so an AI
    agent reads the exit code and a short reason rather than a wall of text.

    Because this calls exit, a script that uses it must be run as its own process (for example
    pwsh -File). A caller that composes such scripts invokes them as child processes and branches
    on the exit code, rather than dot-sourcing or calling them with the call operator.

    .PARAMETER Reason
    The standardized outcome name. Defaults to Failure.

    .PARAMETER Message
    A short, single-line reason. Optional; omit for Success.

    .EXAMPLE
    Stop-Script -Reason Build -Message "dotnet build failed (exit 1)."
    #>
    [CmdletBinding()]
    param(
        [ValidateSet("Success", "Failure", "Usage", "Precondition", "Build", "Test", "Lint", "Git")]
        [string] $Reason = "Failure",

        [string] $Message
    )

    if (-not [string]::IsNullOrWhiteSpace($Message)) {
        # Structured single line so a caller branches on [Reason/Code] and still reads the message.
        $line = if ($Reason -eq "Success") { $Message }
        else { "ERROR [{0}/{1}]: {2}" -f $Reason, (Get-AiKitExitCode -Name $Reason), $Message }
        [Console]::Error.WriteLine($line)
    }
    exit (Get-AiKitExitCode -Name $Reason)
}

function Test-VerboseRequested {
    <#
    .SYNOPSIS
    Determines whether the caller asked for verbose output.

    .DESCRIPTION
    True when -Verbose was passed (or $VerbosePreference is not SilentlyContinue). Scripts use it
    to keep output quiet by default and let the underlying tools inherit the verbosity.

    .EXAMPLE
    $args = if (Test-VerboseRequested) { @() } else { @("-v", "quiet") }
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    param()

    return $VerbosePreference -ne [System.Management.Automation.ActionPreference]::SilentlyContinue
}

function Invoke-Tool {
    <#
    .SYNOPSIS
    Runs an external tool quietly, but surfaces the real diagnostics when it fails.

    .DESCRIPTION
    Keeps output asymmetric — near-silent on success, informative on failure — so an agent pays
    tokens only when something breaks:

      -Verbose        streams the tool live (for a human watching).
      default         captures the output. On success it is discarded (silence). On a non-zero
                      exit, the diagnostically relevant lines (those naming an error or a failure,
                      or the tail when none match) are written to stderr, then the script stops
                      with the standardized code via Stop-Script.

    Quiet hides the noise of success, never the cause of failure.

    .PARAMETER FilePath
    The executable to run (for example "dotnet").

    .PARAMETER Arguments
    The arguments to pass to it.

    .PARAMETER FailReason
    The standardized outcome name used when the tool exits non-zero (see Get-AiKitExitCode).

    .PARAMETER FailMessage
    A short, single-line description of the failure.

    .PARAMETER MaxErrorLines
    Upper bound on the number of failure lines surfaced. Defaults to 40.

    .EXAMPLE
    Invoke-Tool -FilePath dotnet -Arguments "build", $sln, "-v", "quiet" -FailReason Build -FailMessage "dotnet build failed"
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, Position = 0)]
        [ValidateNotNullOrEmpty()]
        [string] $FilePath,

        [string[]] $Arguments = @(),

        [Parameter(Mandatory)]
        [ValidateSet("Failure", "Usage", "Precondition", "Build", "Test", "Lint", "Git")]
        [string] $FailReason,

        [Parameter(Mandatory)]
        [string] $FailMessage,

        [int] $MaxErrorLines = 40
    )

    # Function-local: a non-zero native exit must not throw here — the exit code is inspected.
    $PSNativeCommandUseErrorActionPreference = $false

    if (Test-VerboseRequested) {
        & $FilePath @Arguments
        $code = $LASTEXITCODE
    }
    else {
        $captured = & $FilePath @Arguments 2>&1
        $code = $LASTEXITCODE

        if ($code -ne 0) {
            $lines = @($captured | ForEach-Object { "$_" })
            # Prefer the lines that name an error or failure; fall back to everything.
            $relevant = @($lines | Where-Object { $_ -match '(?i)\berror\b|\bfailed\b|error [A-Za-z]{1,6}\d+' })
            if ($relevant.Count -eq 0) { $relevant = $lines }
            $shown = @($relevant | Select-Object -First $MaxErrorLines)
            $shown | ForEach-Object { [Console]::Error.WriteLine($_) }

            # Steer to the full output rather than silently dropping the rest.
            $hidden = $lines.Count - $shown.Count
            if ($hidden -gt 0) {
                [Console]::Error.WriteLine(("... ({0} more line(s); re-run with -Verbose for the full output)" -f $hidden))
            }
        }
    }

    if ($code -ne 0) {
        Stop-Script -Reason $FailReason -Message ("{0} (exit {1})" -f $FailMessage, $code)
    }
}

# Repository-specific helpers, if this repository has any. Kept in a separate file
# so that this one stays generated and can be replaced on every sync.
$localHelpers = Join-Path $PSScriptRoot "Common.local.ps1"
if (Test-Path -LiteralPath $localHelpers -PathType Leaf) {
    . $localHelpers
}
