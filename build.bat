@echo off
REM Build script for FlegmonCreature mod on Windows
REM You need to set the RIMWORLD_DIR environment variable to your RimWorld installation directory

if "%RIMWORLD_DIR%"=="" (
    echo ERROR: RIMWORLD_DIR environment variable is not set!
    echo Please set it to your RimWorld installation directory.
    echo Example: set RIMWORLD_DIR=C:\Program Files ^(x86^)\Steam\steamapps\common\RimWorld
    echo.
    echo Alternatively, you can edit this script and replace %%RIMWORLD_DIR%% with your actual path.
    exit /b 1
)

echo Building FlegmonCreature mod...
echo Using RimWorld directory: %RIMWORLD_DIR%
dotnet build Source\FlegmonCreature.csproj -p:RimWorldInstallDir="%RIMWORLD_DIR%"

if %ERRORLEVEL% EQU 0 (
    echo Build successful! DLL created in Assemblies folder
) else (
    echo Build failed
    exit /b 1
)