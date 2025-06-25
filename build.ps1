# Build script for FlegmonCreature mod on Windows PowerShell
# You need to set the RIMWORLD_DIR environment variable to your RimWorld installation directory

if (-not $env:RIMWORLD_DIR) {
    Write-Host "ERROR: RIMWORLD_DIR environment variable is not set!" -ForegroundColor Red
    Write-Host "Please set it to your RimWorld installation directory."
    Write-Host "Example: `$env:RIMWORLD_DIR = 'C:\Program Files (x86)\Steam\steamapps\common\RimWorld'"
    Write-Host ""
    Write-Host "Alternatively, you can edit this script and replace `$env:RIMWORLD_DIR with your actual path."
    exit 1
}

Write-Host "Building FlegmonCreature mod..." -ForegroundColor Green
Write-Host "Using RimWorld directory: $env:RIMWORLD_DIR"

$buildResult = dotnet build "Source\FlegmonCreature.csproj" -p:RimWorldInstallDir="$env:RIMWORLD_DIR"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! DLL created in Assemblies folder" -ForegroundColor Green
} else {
    Write-Host "Build failed" -ForegroundColor Red
    exit 1
}