# Build verification script
$rimWorldPath = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"

Write-Host "Checking build prerequisites..." -ForegroundColor Cyan

# Check if .NET SDK is installed
Write-Host "`nChecking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "[OK] .NET SDK found: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Check if RimWorld path exists
Write-Host "`nChecking RimWorld installation..." -ForegroundColor Yellow
if (Test-Path $rimWorldPath) {
    Write-Host "[OK] RimWorld installation found at: $rimWorldPath" -ForegroundColor Green
} else {
    Write-Host "[ERROR] RimWorld not found at expected location!" -ForegroundColor Red
    Write-Host "Please update the path in build scripts" -ForegroundColor Yellow
}

# Check for Assembly-CSharp.dll
$assemblyPath = Join-Path $rimWorldPath "RimWorldWin64_Data\Managed\Assembly-CSharp.dll"
if (Test-Path $assemblyPath) {
    Write-Host "[OK] Assembly-CSharp.dll found" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Assembly-CSharp.dll not found!" -ForegroundColor Red
    Write-Host "Expected at: $assemblyPath" -ForegroundColor Yellow
}

# List all C# files in Source directory
Write-Host "`nC# files in Source directory:" -ForegroundColor Yellow
Get-ChildItem -Path "Source" -Filter "*.cs" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor Cyan
}

# Try a simple build
Write-Host "`nAttempting build..." -ForegroundColor Yellow
Set-Location "Source"
$buildArgs = @(
    "build",
    "-p:RimWorldInstallDir=`"$rimWorldPath`"",
    "-v:n"
)

& dotnet $buildArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[OK] Build succeeded!" -ForegroundColor Green
} else {
    Write-Host "`n[ERROR] Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
}

Set-Location ..