# Clean build script for FlegmonCreature mod

Write-Host "Cleaning build cache..." -ForegroundColor Yellow

# Remove obj directory
if (Test-Path "Source\obj") {
    Remove-Item -Path "Source\obj" -Recurse -Force
    Write-Host "[OK] Removed obj directory" -ForegroundColor Green
}

# Remove bin directory if exists
if (Test-Path "Source\bin") {
    Remove-Item -Path "Source\bin" -Recurse -Force
    Write-Host "[OK] Removed bin directory" -ForegroundColor Green
}

# Clean Assemblies directory
if (Test-Path "Assemblies") {
    Remove-Item -Path "Assemblies\*" -Force
    Write-Host "[OK] Cleaned Assemblies directory" -ForegroundColor Green
}

Write-Host "`nBuild cache cleaned successfully!" -ForegroundColor Green
Write-Host "You can now run 'dotnet build' or 'build.ps1' to rebuild the project." -ForegroundColor Cyan