<#
.SYNOPSIS
    Installs, updates, and configures the unified com.unity.pipeline package and unity-cli skill.
    Supports Unity 2018.1, 2019.4 LTS, 2022.3 LTS, and Unity 6000.0+.

.PARAMETER ProjectPath
    Absolute or relative path to the Unity project root. Defaults to current directory.

.PARAMETER Port
    TCP port for the Unity Pipeline RPC server. Default: 7800.

.PARAMETER SourceType
    Installation mechanism: 'Git' (default for 2019+), 'LocalPath', 'Link' (junction), or 'Registry'.
    On Unity 2018.1/2018.2 (where UPM lacks git/file support), automatically links/copies into Assets/com.unity.pipeline.

.PARAMETER GitUrl
    Git repository URL for com.unity.pipeline.
    Default: 'https://github.com/SmittyWerbenJJ/unity-cli.git'

.PARAMETER LocalPath
    Path to local com.unity.pipeline package directory. Defaults to this skill repository root.

.PARAMETER Update
    Pulls latest updates from GitHub (git pull) and syncs the IvanMurzak feature tracker via $TEMP\unity-mcp-sync.

.PARAMETER LaunchDaemon
    If specified, launches Unity.exe in headless batchmode daemon mode (omitting -quit).
#>

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$ProjectPath = ".",

    [Parameter()]
    [int]$Port = 7800,

    [Parameter()]
    [ValidateSet("Git", "LocalPath", "Link", "Registry")]
    [string]$SourceType = "Git",

    [Parameter()]
    [string]$GitUrl = "https://github.com/SmittyWerbenJJ/unity-cli.git",

    [Parameter()]
    [string]$LocalPath = (Split-Path -Parent $PSScriptRoot),

    [Parameter()]
    [switch]$Update,

    [Parameter()]
    [switch]$LaunchDaemon,

    [Parameter()]
    [string]$EditorPath = ""
)

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent $PSScriptRoot

function Write-Step([string]$Message) { Write-Host "[+] $Message" -ForegroundColor Cyan }
function Write-Success([string]$Message) { Write-Host "[OK] $Message" -ForegroundColor Green }
function Write-Warn([string]$Message) { Write-Host "[!] $Message" -ForegroundColor Yellow }
function Write-Err([string]$Message) { Write-Host "[ERROR] $Message" -ForegroundColor Red }

if ($Update) {
    Write-Step "Updating unified skill & Unity package repository at $RepoRoot..."
    Push-Location $RepoRoot
    try {
        git pull --ff-only
        Write-Success "Git repository updated."
        $syncScript = Join-Path $PSScriptRoot "sync_ivanmurzak_tracker.py"
        if (Test-Path $syncScript) {
            Write-Step "Checking for IvanMurzak/Unity-MCP updates in `$env:TEMP\unity-mcp-sync..."
            python $syncScript
        }
    }
    finally {
        Pop-Location
    }
    if ($ProjectPath -eq "." -and -not (Test-Path (Join-Path $ProjectPath "ProjectSettings"))) {
        exit 0
    }
}

$resolvedProjectPath = [System.IO.Path]::GetFullPath($ProjectPath)
Write-Step "Target Unity project: $resolvedProjectPath"

$assetsPath = Join-Path $resolvedProjectPath "Assets"
$projectSettingsPath = Join-Path $resolvedProjectPath "ProjectSettings"
if (-not (Test-Path $assetsPath) -and -not (Test-Path $projectSettingsPath)) {
    Write-Err "Directory does not appear to be a Unity project (missing Assets/ProjectSettings): $resolvedProjectPath"
    exit 1
}

$versionFile = Join-Path $projectSettingsPath "ProjectVersion.txt"
$unityVersion = "Unknown"
if (Test-Path $versionFile) {
    $versionContent = Get-Content $versionFile -Raw
    if ($versionContent -match "m_EditorVersion:\s*([^\r\n]+)") {
        $unityVersion = $Matches[1].Trim()
        Write-Step "Detected Unity Editor version: $unityVersion"
    }
}

$packagesDir = Join-Path $resolvedProjectPath "Packages"
if (-not (Test-Path $packagesDir)) {
    New-Item -ItemType Directory -Path $packagesDir -Force | Out-Null
}
$manifestPath = Join-Path $packagesDir "manifest.json"

# Unity 2018.1 and 2018.2 UPM does not support git: or file: packages in Packages/manifest.json;
# install into Assets/com.unity.pipeline where PipelineBootstrap.cs also unpacks Assets/Plugins/Newtonsoft.Json.dll.
if ($unityVersion -match "^2018\.[12]\.") {
    $resolvedLocalPath = [System.IO.Path]::GetFullPath($LocalPath)
    $targetAssetsPkg = Join-Path $assetsPath "com.unity.pipeline"
    if (Test-Path $targetAssetsPkg) {
        Remove-Item $targetAssetsPkg -Recurse -Force
    }
    Write-Step "Unity $unityVersion detected: linking com.unity.pipeline into $targetAssetsPkg"
    cmd /c mklink /J "$targetAssetsPkg" "$resolvedLocalPath" | Out-Null
    Write-Success "Linked com.unity.pipeline into Assets/com.unity.pipeline (Zero-Touch Bootstrapper active)."
}
else {
    switch ($SourceType) {
        "Registry" {
            Write-Step "Installing com.unity.pipeline via unity-cli / UPM registry..."
            & unity pipeline install --project-path $resolvedProjectPath --format json
        }
        "Link" {
            $resolvedLocalPath = [System.IO.Path]::GetFullPath($LocalPath)
            $targetLink = Join-Path $packagesDir "com.unity.pipeline"
            if (Test-Path $targetLink) { Remove-Item $targetLink -Recurse -Force }
            cmd /c mklink /J "$targetLink" "$resolvedLocalPath" | Out-Null
            Write-Success "Linked $targetLink -> $resolvedLocalPath"
        }
        default {
            $packageRef = if ($SourceType -eq "Git") { $GitUrl } else { "file:" + [System.IO.Path]::GetFullPath($LocalPath).Replace("\", "/") }
            Write-Step "Configuring com.unity.pipeline ($SourceType): $packageRef"
            if (Test-Path $manifestPath) {
                $manifestObj = (Get-Content $manifestPath -Raw -Encoding UTF8) | ConvertFrom-Json
                Copy-Item $manifestPath "$manifestPath.bak" -Force
                $deps = [ordered]@{}
                if ($manifestObj.dependencies) {
                    foreach ($prop in $manifestObj.dependencies.PSObject.Properties) { $deps[$prop.Name] = $prop.Value }
                }
                $deps["com.unity.pipeline"] = $packageRef
                $manifestHash = [ordered]@{}
                foreach ($prop in $manifestObj.PSObject.Properties) {
                    $manifestHash[$prop.Name] = if ($prop.Name -eq "dependencies") { $deps } else { $prop.Value }
                }
                if (-not $manifestHash.Contains("dependencies")) { $manifestHash["dependencies"] = $deps }
                [System.IO.File]::WriteAllText($manifestPath, ($manifestHash | ConvertTo-Json -Depth 10), [System.Text.Encoding]::UTF8)
            }
            else {
                $manifestHash = [ordered]@{ dependencies = [ordered]@{ "com.unity.pipeline" = $packageRef } }
                [System.IO.File]::WriteAllText($manifestPath, ($manifestHash | ConvertTo-Json -Depth 10), [System.Text.Encoding]::UTF8)
            }
            Write-Success "Updated $manifestPath"
        }
    }
}

$portFile = Join-Path $resolvedProjectPath ".unity-pipeline-port"
[System.IO.File]::WriteAllText($portFile, $Port.ToString().Trim(), [System.Text.Encoding]::ASCII)
Write-Success "Configured discovery port: $portFile ($Port)"
