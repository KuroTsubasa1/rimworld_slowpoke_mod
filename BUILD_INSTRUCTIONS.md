# Build-Anweisungen für FlegmonCreature Mod

## Problem
Die RimWorld DLL-Referenzen können nicht gefunden werden.

## Lösung

### Option 1: Umgebungsvariable setzen (Empfohlen)
1. Öffnen Sie PowerShell als Administrator
2. Führen Sie einen der folgenden Befehle aus (je nach Ihrem RimWorld-Installationspfad):

```powershell
# Für Steam-Installation in Program Files (x86)
$env:RIMWORLD_DIR = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"
[System.Environment]::SetEnvironmentVariable("RIMWORLD_DIR", $env:RIMWORLD_DIR, "User")

# Für Steam-Installation in Program Files
$env:RIMWORLD_DIR = "C:\Program Files\Steam\steamapps\common\RimWorld"
[System.Environment]::SetEnvironmentVariable("RIMWORLD_DIR", $env:RIMWORLD_DIR, "User")

# Für Steam-Installation auf D:\
$env:RIMWORLD_DIR = "D:\Steam\steamapps\common\RimWorld"
[System.Environment]::SetEnvironmentVariable("RIMWORLD_DIR", $env:RIMWORLD_DIR, "User")
```

3. Dann bauen Sie das Projekt:
```powershell
.\build.ps1
```

### Option 2: Direkter Build-Befehl
Wenn Sie RimWorld an einem der Standard-Orte installiert haben, können Sie direkt bauen:

```powershell
# Beispiel für C:\Program Files (x86)\Steam
dotnet build "Source\FlegmonCreature.csproj" -p:RimWorldInstallDir="C:\Program Files (x86)\Steam\steamapps\common\RimWorld"
```

### Option 3: Manuelle DLL-Kopie
1. Finden Sie Ihren RimWorld-Installationsordner
2. Navigieren Sie zu `RimWorldWin64_Data\Managed`
3. Erstellen Sie einen Ordner `lib` im Source-Verzeichnis
4. Kopieren Sie diese DLLs in den lib-Ordner:
   - Assembly-CSharp.dll
   - UnityEngine.CoreModule.dll

## Fehlerbehebung
Wenn der Build immer noch fehlschlägt:
1. Stellen Sie sicher, dass RimWorld installiert ist
2. Überprüfen Sie, ob die DLLs am angegebenen Pfad existieren
3. Starten Sie PowerShell/Ihre IDE neu nach dem Setzen der Umgebungsvariable