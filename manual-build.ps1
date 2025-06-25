# Manual build script for FlegmonCreature mod
$ErrorActionPreference = "Stop"

$rimWorldPath = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"
$outputPath = "Assemblies"
$sourcePath = "Source"

Write-Host "Manual build script for FlegmonCreature mod" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan

# Create Assemblies directory if it doesn't exist
if (!(Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
    Write-Host "Created Assemblies directory" -ForegroundColor Green
}

# Get all C# files
$sourceFiles = Get-ChildItem -Path $sourcePath -Filter "*.cs" -Recurse | Select-Object -ExpandProperty FullName

Write-Host "`nFound $($sourceFiles.Count) source files:" -ForegroundColor Yellow
$sourceFiles | ForEach-Object { Write-Host "  - $(Split-Path $_ -Leaf)" -ForegroundColor Gray }

# Build reference paths
$references = @(
    "-r:`"$rimWorldPath\RimWorldWin64_Data\Managed\Assembly-CSharp.dll`"",
    "-r:`"$rimWorldPath\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll`"",
    "-r:`"$rimWorldPath\RimWorldWin64_Data\Managed\UnityEngine.dll`"",
    "-r:`"$rimWorldPath\RimWorldWin64_Data\Managed\UnityEngine.IMGUIModule.dll`""
)

# Build command
$cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if (!(Test-Path $cscPath)) {
    Write-Host "C# compiler not found at: $cscPath" -ForegroundColor Red
    Write-Host "Trying to use Roslyn compiler instead..." -ForegroundColor Yellow
    
    # Try to compile using dotnet build as fallback
    Write-Host "`nFalling back to dotnet build..." -ForegroundColor Yellow
    Set-Location $sourcePath
    & dotnet build -p:RimWorldInstallDir="$rimWorldPath"
    Set-Location ..
    exit $LASTEXITCODE
}

Write-Host "`nCompiling with csc.exe..." -ForegroundColor Yellow

$compileArgs = @(
    "/target:library",
    "/out:`"$outputPath\FlegmonCreature.dll`"",
    "/nologo"
) + $references + $sourceFiles

& $cscPath $compileArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[OK] Build succeeded!" -ForegroundColor Green
    Write-Host "Output: $outputPath\FlegmonCreature.dll" -ForegroundColor Green
} else {
    Write-Host "`n[ERROR] Build failed!" -ForegroundColor Red
}

exit $LASTEXITCODE