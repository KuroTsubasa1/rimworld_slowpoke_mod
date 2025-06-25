# Test for duplicate class definitions

Write-Host "Checking for duplicate class definitions..." -ForegroundColor Yellow

$sourceFiles = Get-ChildItem -Path "Source" -Filter "*.cs" -File

$classDefinitions = @{}

foreach ($file in $sourceFiles) {
    $content = Get-Content $file.FullName -Raw
    $matches = [regex]::Matches($content, 'public\s+class\s+(\w+)\s*:')
    
    foreach ($match in $matches) {
        $className = $match.Groups[1].Value
        if ($classDefinitions.ContainsKey($className)) {
            Write-Host "[DUPLICATE] Class '$className' found in:" -ForegroundColor Red
            Write-Host "  - $($classDefinitions[$className])" -ForegroundColor Yellow
            Write-Host "  - $($file.Name)" -ForegroundColor Yellow
        } else {
            $classDefinitions[$className] = $file.Name
        }
    }
}

Write-Host "`nClass definitions found:" -ForegroundColor Green
foreach ($kvp in $classDefinitions.GetEnumerator() | Sort-Object Name) {
    Write-Host "  $($kvp.Key) in $($kvp.Value)" -ForegroundColor Gray
}

Write-Host "`nDuplicate check complete!" -ForegroundColor Green