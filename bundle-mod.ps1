# Script to bundle the FlegmonCreature mod into a zip file
# This script creates a clean mod package ready for distribution

$modName = "FlegmonCreature"
$version = "1.0.0"
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$zipName = "${modName}-v${version}-${timestamp}.zip"

# Check if required folders exist
if (-not (Test-Path "About")) {
    Write-Host "ERROR: About folder not found!" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path "Defs")) {
    Write-Host "ERROR: Defs folder not found!" -ForegroundColor Red
    exit 1
}

Write-Host "Creating mod bundle: $zipName" -ForegroundColor Green

# Create a temporary folder for the clean mod structure
$tempFolder = "temp_bundle_$timestamp"
$modFolder = Join-Path $tempFolder $modName

if (Test-Path $tempFolder) {
    Remove-Item $tempFolder -Recurse -Force
}

New-Item -ItemType Directory -Path $modFolder -Force | Out-Null

# Copy essential mod files and folders
Write-Host "Copying mod files..." -ForegroundColor Yellow

# About folder (required)
if (Test-Path "About") {
    Copy-Item -Path "About" -Destination $modFolder -Recurse
    Write-Host "  ✓ About folder copied" -ForegroundColor Green
}

# Assemblies folder (compiled DLLs)
if (Test-Path "Assemblies") {
    Copy-Item -Path "Assemblies" -Destination $modFolder -Recurse
    Write-Host "  ✓ Assemblies folder copied" -ForegroundColor Green
}

# Defs folder (required)
if (Test-Path "Defs") {
    Copy-Item -Path "Defs" -Destination $modFolder -Recurse
    Write-Host "  ✓ Defs folder copied" -ForegroundColor Green
}

# Languages folder (if exists)
if (Test-Path "Languages") {
    Copy-Item -Path "Languages" -Destination $modFolder -Recurse
    Write-Host "  ✓ Languages folder copied" -ForegroundColor Green
}

# Textures folder (if exists)
if (Test-Path "Textures") {
    Copy-Item -Path "Textures" -Destination $modFolder -Recurse
    Write-Host "  ✓ Textures folder copied" -ForegroundColor Green
}

# Copy README if exists
if (Test-Path "README.md") {
    Copy-Item -Path "README.md" -Destination $modFolder
    Write-Host "  ✓ README.md copied" -ForegroundColor Green
}

# Create the zip file
Write-Host "\nCreating zip archive..." -ForegroundColor Yellow

try {
    # Check if we have .NET compression available
    Add-Type -AssemblyName System.IO.Compression.FileSystem -ErrorAction Stop
    
    # Create the zip file
    [System.IO.Compression.ZipFile]::CreateFromDirectory($tempFolder, $zipName)
    
    Write-Host "✓ Mod bundled successfully: $zipName" -ForegroundColor Green
    Write-Host "  Size: $([math]::Round((Get-Item $zipName).Length / 1MB, 2)) MB" -ForegroundColor Cyan
}
catch {
    Write-Host "Using PowerShell Compress-Archive as fallback..." -ForegroundColor Yellow
    Compress-Archive -Path (Join-Path $tempFolder "*") -DestinationPath $zipName -Force
    Write-Host "✓ Mod bundled successfully: $zipName" -ForegroundColor Green
}

# Clean up temporary folder
Remove-Item $tempFolder -Recurse -Force

Write-Host "\nBundle created!" -ForegroundColor Green
Write-Host "Location: $(Get-Location)\$zipName" -ForegroundColor Cyan
Write-Host "\nThe mod is ready for distribution!" -ForegroundColor Green