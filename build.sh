#!/bin/bash

# Build script for FlegmonCreature mod
# You need to set the RIMWORLD_DIR environment variable to your RimWorld installation directory

if [ -z "$RIMWORLD_DIR" ]; then
    echo "Error: RIMWORLD_DIR environment variable is not set"
    echo "Please set it to your RimWorld installation directory"
    echo "Example: export RIMWORLD_DIR=\"/path/to/RimWorld\""
    exit 1
fi

echo "Building FlegmonCreature mod..."
dotnet build Source/FlegmonCreature.csproj -p:RimWorldInstallDir="$RIMWORLD_DIR"

if [ $? -eq 0 ]; then
    echo "Build successful! DLL created in Assemblies folder"
else
    echo "Build failed"
    exit 1
fi