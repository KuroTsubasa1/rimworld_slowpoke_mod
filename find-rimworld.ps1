# Script to find RimWorld installation and set RIMWORLD_DIR environment variable

$possiblePaths = @(
    "C:\Program Files (x86)\Steam\steamapps\common\RimWorld",
    "C:\Program Files\Steam\steamapps\common\RimWorld",
    "D:\Steam\steamapps\common\RimWorld",
    "D:\SteamLibrary\steamapps\common\RimWorld",
    "E:\Steam\steamapps\common\RimWorld",
    "E:\SteamLibrary\steamapps\common\RimWorld"
)

$rimworldPath = $null

foreach ($path in $possiblePaths) {
    if (Test-Path "$path\RimWorldWin64_Data\Managed\Assembly-CSharp.dll") {
        $rimworldPath = $path
        break
    }
}

if ($rimworldPath) {
    Write-Host "Found RimWorld at: $rimworldPath" -ForegroundColor Green
    $env:RIMWORLD_DIR = $rimworldPath
    [System.Environment]::SetEnvironmentVariable("RIMWORLD_DIR", $rimworldPath, "User")
    Write-Host "RIMWORLD_DIR environment variable has been set!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run the build script:" -ForegroundColor Yellow
    Write-Host "  .\build.ps1"
} else {
    Write-Host "ERROR: Could not find RimWorld installation!" -ForegroundColor Red
    Write-Host "Please install RimWorld or manually set RIMWORLD_DIR to your installation path."
    Write-Host "Example: `$env:RIMWORLD_DIR = 'C:\Your\Path\To\RimWorld'"
    exit 1
}