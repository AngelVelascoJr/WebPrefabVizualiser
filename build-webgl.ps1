# Build WebGL to docs/ (close the Unity Editor first)
$ErrorActionPreference = "Stop"
$unity = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe"
$project = $PSScriptRoot
$lock = Join-Path $project "Temp\UnityLockfile"

if (Test-Path $lock) {
    Write-Host "Close the Unity Editor for this project, then run this script again."
    exit 1
}

if (-not (Test-Path $unity)) {
    Write-Error "Unity not found: $unity"
}

& $unity -batchmode -quit -nographics `
    -projectPath $project `
    -executeMethod PrefabViewer.Editor.PrefabViewerSetup.SetupAndBuildBatch `
    -logFile (Join-Path $project "build.log")

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. See build.log"
    exit $LASTEXITCODE
}

Write-Host "OK: docs/ ready for GitHub Pages"
