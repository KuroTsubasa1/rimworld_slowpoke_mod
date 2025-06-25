# Script to find IntVec3 to TargetInfo conversion issues

$sourceDir = "Source"
$files = Get-ChildItem -Path $sourceDir -Filter "*.cs" -Recurse

Write-Host "Searching for potential IntVec3 to TargetInfo conversion issues..." -ForegroundColor Yellow

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $lines = $content -split "`n"
    
    for ($i = 0; $i -lt $lines.Length; $i++) {
        $line = $lines[$i]
        $lineNum = $i + 1
        
        # Check for Messages.Message calls with potential issues
        if ($line -match "Messages\.Message" -and $line -match "pawn\.Position" -and $line -notmatch "new TargetInfo") {
            Write-Host "`nFound potential issue in $($file.Name):" -ForegroundColor Red
            Write-Host "Line $lineNum`: $line" -ForegroundColor Yellow
        }
        
        # Check for other potential TargetInfo conversions
        if ($line -match "TargetInfo" -and $line -match "IntVec3" -and $line -notmatch "new TargetInfo") {
            Write-Host "`nFound potential conversion in $($file.Name):" -ForegroundColor Red
            Write-Host "Line $lineNum`: $line" -ForegroundColor Yellow
        }
        
        # Check for implicit conversions in method calls
        if ($line -match "\([^)]*pawn\.Position[^)]*\)" -and $line -notmatch "new TargetInfo") {
            # Exclude known safe methods
            if ($line -notmatch "GenSpawn\.Spawn" -and 
                $line -notmatch "GenAdj\." -and 
                $line -notmatch "cell\.InBounds" -and
                $line -notmatch "ThingMaker\.MakeThing") {
                Write-Host "`nFound method call with pawn.Position in $($file.Name):" -ForegroundColor Cyan
                Write-Host "Line $lineNum`: $line" -ForegroundColor Yellow
            }
        }
    }
}

Write-Host "`nSearch complete!" -ForegroundColor Green