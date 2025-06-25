# Simple build test script
$ErrorActionPreference = "Stop"

Write-Host "Testing FlegmonCreature build..." -ForegroundColor Yellow

try {
    # Navigate to source directory
    Push-Location -Path "Source"
    
    # Run build
    $buildResult = & dotnet build FlegmonCreature.csproj --verbosity minimal 2>&1
    
    # Check for errors
    $errors = $buildResult | Where-Object { $_ -match "error CS" }
    
    if ($errors) {
        Write-Host "[ERROR] Build failed with the following errors:" -ForegroundColor Red
        $errors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        exit 1
    } else {
        Write-Host "[OK] Build completed successfully!" -ForegroundColor Green
        
        # Check if DLL was created
        if (Test-Path "..\Assemblies\FlegmonCreature.dll") {
            Write-Host "[OK] FlegmonCreature.dll found in Assemblies folder" -ForegroundColor Green
        } else {
            Write-Host "[WARNING] FlegmonCreature.dll not found in Assemblies folder" -ForegroundColor Yellow
        }
    }
    
} catch {
    Write-Host "[ERROR] An exception occurred: $_" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}

Write-Host "`nBuild test complete!" -ForegroundColor Green