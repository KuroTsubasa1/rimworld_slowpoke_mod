# Direct build script with hardcoded RimWorld path
$rimworldPath = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"

Write-Host "Building FlegmonCreature mod..." -ForegroundColor Green
Write-Host "Using RimWorld directory: $rimworldPath"

# Change to project directory
Set-Location "c:\Users\lasse\PhpstormProjects\rimworld_slowpoke_mod"

# Run the build command
& dotnet build "Source\FlegmonCreature.csproj" "-p:RimWorldInstallDir=$rimworldPath"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! DLL created in Assemblies folder" -ForegroundColor Green
} else {
    Write-Host "Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
}