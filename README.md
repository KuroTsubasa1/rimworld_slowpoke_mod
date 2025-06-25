# Flegmon Creature Mod

Adds the Flegmon, a friendly pink lizard creature to RimWorld.

## Building the Mod

### Prerequisites
- .NET SDK (6.0 or later)
- RimWorld game installed

### Option 1: Using Command Line

1. Set your RimWorld installation directory:
   - **Windows**: `set RIMWORLD_DIR=C:\Program Files (x86)\Steam\steamapps\common\RimWorld`
   - **Mac/Linux**: `export RIMWORLD_DIR=/path/to/RimWorld`

2. Run the build script:
   - **Windows**: `build.bat`
   - **Mac/Linux**: `./build.sh`

### Option 2: Manual Build

```bash
cd Source
dotnet build FlegmonCreature.csproj -p:RimWorldInstallDir="[YOUR_RIMWORLD_PATH]"
```

### Option 3: Without Compiling

If you don't want to compile, you can remove the meat-dropping feature:
1. Delete the `Source` folder
2. Remove this line from `Defs/ThingDefs_Races/Races_Animal_Flegmon.xml`:
   ```xml
   <li Class="FlegmonCreature.CompProperties_FlegmonTailDrop" />
   ```

The mod will still work but Flegmons won't drop tail meat periodically.

## Installation

Copy the entire `FlegmonCreature` folder to your RimWorld Mods directory:
- **Windows**: `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Mods\`
- **Mac**: `~/Library/Application Support/RimWorld/Mods/`
- **Linux**: `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Mods/`