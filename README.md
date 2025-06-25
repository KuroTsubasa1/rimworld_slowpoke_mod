# Flegmon Creature Mod

Ein RimWorld-Mod, der die Flegmon-Kreatur zum Spiel hinzufügt.

## Features

### Basic Features
- Custom creature: Flegmon
- Shearable wool production
- Tail meat dropping mechanism

### Extended Features (New)
- **Water Detection**: Flegmons can detect and mark water sources on the map
- **Water-Loving Behavior**: Flegmons need regular baths and get mood buffs when wet
- **Enhanced Tail System**: 
  - Visual tail stages (Stub, Growing, Full)
  - 15% chance to drop premium tail meat
  - Tail regrowth system
- **Social Interactions**:
  - Mood therapy aura that buffs nearby colonists
  - Cuddle-seeking behavior
- **Special Abilities**:
  - Rain Dance: Can summon rain during droughts
  - Slime Trail: Leaves a trail that slows enemies

## Voraussetzungen

1. **RimWorld** muss installiert sein
2. **.NET SDK** (Version 6.0 oder höher) - [Download von Microsoft](https://dotnet.microsoft.com/download)

## Build-Anleitung

### Schritt 1: RimWorld-Pfad festlegen

Bevor Sie das Projekt kompilieren können, müssen Sie den Pfad zu Ihrer RimWorld-Installation angeben.

**Option A: Umgebungsvariable setzen (empfohlen)**
```cmd
set RIMWORLD_DIR=C:\Program Files (x86)\Steam\steamapps\common\RimWorld
```

**Option B: Build-Skript anpassen**

Bearbeiten Sie `build.bat` und ersetzen Sie `%RIMWORLD_DIR%` mit Ihrem tatsächlichen RimWorld-Pfad.

### Schritt 2: Mod kompilieren

```cmd
cd c:\Users\lasse\PhpstormProjects\rimworld_slowpoke_mod
build.bat
```

## Häufige Probleme

### "Assembly-CSharp" wurde nicht gefunden

Dieser Fehler tritt auf, wenn der RimWorld-Pfad nicht korrekt gesetzt ist. Überprüfen Sie:

1. Dass RimWorld installiert ist
2. Dass der Pfad in der Umgebungsvariable `RIMWORLD_DIR` korrekt ist
3. Dass die folgenden Dateien existieren:
   - `%RIMWORLD_DIR%\RimWorldWin64_Data\Managed\Assembly-CSharp.dll`
   - `%RIMWORLD_DIR%\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll`

### Typische RimWorld-Installationspfade

- **Steam**: `C:\Program Files (x86)\Steam\steamapps\common\RimWorld`
- **GOG**: `C:\GOG Games\RimWorld`
- **Epic Games**: `C:\Program Files\Epic Games\RimWorld`

## Mod-Struktur

- `/About/` - Mod-Informationen
- `/Assemblies/` - Kompilierte DLL (wird nach dem Build erstellt)
- `/Defs/` - XML-Definitionen für das Spiel
- `/Source/` - C# Quellcode
- `/Textures/` - Grafiken für den Mod

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

## Component Details

### Water Detection & Water-Loving
- Flegmons scan for water sources every 10 hours
- They mark discovered water sources on the map
- Need to bathe every 2 days or they get unhappy
- Get a "Wet" buff after bathing (+5 mood for 1 day)

### Enhanced Tail System
- Tail has 3 visual stages: Stub → Growing → Full
- Can only drop tail meat when fully grown
- 15% chance for premium quality tail meat
- Takes 7 days to fully regrow after dropping

### Social Features
- Flegmons emit a calming aura (+3 mood to nearby colonists)
- Seek cuddles from colonists when lonely
- Cuddled colonists get +8 mood for 12 hours

### Special Abilities
- **Rain Dance**: 30% success chance, 7-day cooldown
- **Slime Trail**: Leaves slippery trails that slow movement, 1-day cooldown

## Installation

Copy the entire `FlegmonCreature` folder to your RimWorld Mods directory:
- **Windows**: `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Mods\`
- **Mac**: `~/Library/Application Support/RimWorld/Mods/`
- **Linux**: `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Mods/`