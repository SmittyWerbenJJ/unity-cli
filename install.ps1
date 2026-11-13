param(
    [string]$TargetDir = (Join-Path $HOME ".gemini\config\skills\unity-cli")
)

$ErrorActionPreference = "Stop"
$RepoUrl = "https://github.com/SmittyWerbenJJ/unity-cli.git"

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "Git is required to install unity-cli. Install Git and re-run this script."
}

if (Test-Path (Join-Path $TargetDir ".git")) {
    Write-Host "Updating unity-cli skill in $TargetDir..."
    git -C $TargetDir pull --ff-only
} else {
    $parent = Split-Path -Parent $TargetDir
    if ($parent -and -not (Test-Path $parent)) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }
    Write-Host "Cloning unity-cli skill into $TargetDir..."
    git clone $RepoUrl $TargetDir
}

if ($LASTEXITCODE -ne 0) {
    throw "Failed to install or update unity-cli skill (exit code $LASTEXITCODE)."
}

Write-Host "unity-cli skill is ready at $TargetDir."
