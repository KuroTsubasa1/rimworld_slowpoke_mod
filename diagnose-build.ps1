# Diagnostic script for build issues
$ErrorActionPreference = "Continue"

Write-Host "FlegmonCreature Build Diagnostics" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# 1. Check working directory
Write-Host "`n1. Current Directory:" -ForegroundColor Yellow
Write-Host "   $(Get-Location)" -ForegroundColor Gray

# 2. Check project structure
Write-Host "`n2. Project Structure:" -ForegroundColor Yellow
$folders = @("Source", "Assemblies", "Defs", "Languages")
foreach ($folder in $folders) {
    if (Test-Path $folder) {
        Write-Host "   [OK] $folder/" -ForegroundColor Green
    } else {
        Write-Host "   [ERROR] $folder/ (missing)" -ForegroundColor Red
    }
}

# 3. List source files
Write-Host "`n3. Source Files:" -ForegroundColor Yellow
if (Test-Path "Source") {
    Get-ChildItem -Path "Source" -Filter "*.cs" | ForEach-Object {
        Write-Host "   - $($_.Name) ($($_.Length) bytes)" -ForegroundColor Gray
    }
} else {
    Write-Host "   [ERROR] Source directory not found!" -ForegroundColor Red
}

# 4. Check .csproj file
Write-Host "`n4. Project File:" -ForegroundColor Yellow
$csprojPath = "Source\FlegmonCreature.csproj"
if (Test-Path $csprojPath) {
    Write-Host "   [OK] $csprojPath exists" -ForegroundColor Green
} else {
    Write-Host "   [ERROR] $csprojPath not found!" -ForegroundColor Red
}

# 5. .NET SDK check
Write-Host "`n5. .NET SDK:" -ForegroundColor Yellow
try {
    $dotnetVersion = & dotnet --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   [OK] Version: $dotnetVersion" -ForegroundColor Green
    } else {
        Write-Host "   [ERROR] .NET SDK error" -ForegroundColor Red
    }
} catch {
    Write-Host "   [ERROR] .NET SDK not found or not accessible" -ForegroundColor Red
}

# 6. RimWorld Path Check
Write-Host "`n6. RimWorld Installation:" -ForegroundColor Yellow
$rimWorldPath = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"
if (Test-Path $rimWorldPath) {
    Write-Host "   [OK] Found at: $rimWorldPath" -ForegroundColor Green
    
    # Check for key DLLs
    $requiredDlls = @(
        "RimWorldWin64_Data\Managed\Assembly-CSharp.dll",
        "RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll"
    )
    
    foreach ($dll in $requiredDlls) {
        $dllPath = Join-Path $rimWorldPath $dll
        if (Test-Path $dllPath) {
            Write-Host "   [OK] $dll" -ForegroundColor Green
        } else {
            Write-Host "   [ERROR] $dll (missing)" -ForegroundColor Red
        }
    }
} else {
    Write-Host "   [ERROR] RimWorld not found at expected location" -ForegroundColor Red
    Write-Host "   Please check your RimWorld installation path" -ForegroundColor Yellow
}

# 7. Try a minimal build
Write-Host "`n7. Attempting minimal build..." -ForegroundColor Yellow
if ((Test-Path "Source") -and (Test-Path $csprojPath)) {
    Push-Location "Source"
    Write-Host "   Running: dotnet build --verbosity minimal" -ForegroundColor Gray
    & dotnet build --verbosity minimal -p:RimWorldInstallDir="$rimWorldPath" 2>&1 | Out-String | Write-Host
    $buildResult = $LASTEXITCODE
    Pop-Location
    
    if ($buildResult -eq 0) {
        Write-Host "   [OK] Build succeeded!" -ForegroundColor Green
    } else {
        Write-Host "   [ERROR] Build failed with exit code: $buildResult" -ForegroundColor Red
    }
} else {
    Write-Host "   [ERROR] Cannot attempt build - missing Source directory or project file" -ForegroundColor Red
}

Write-Host "`nDiagnostics complete." -ForegroundColor Cyan