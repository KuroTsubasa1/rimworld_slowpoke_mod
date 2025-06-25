# Script to check line 108 position 63 in CompFlegmonAbilities.cs

$file = "Source\CompFlegmonAbilities.cs"
$content = Get-Content $file

Write-Host "Checking line 108 of $file" -ForegroundColor Yellow
Write-Host "Total lines in file: $($content.Length)"
Write-Host "`nLine 108 content:" -ForegroundColor Cyan

if ($content.Length -ge 108) {
    $line108 = $content[107]  # Arrays are 0-indexed
    Write-Host $line108
    
    Write-Host "`nCharacter at position 63:" -ForegroundColor Cyan
    if ($line108.Length -ge 63) {
        $char63 = $line108.Substring(62, 1)  # Strings are 0-indexed
        Write-Host "Character: '$char63'"
        Write-Host "Context: '$($line108.Substring([Math]::Max(0, 52), [Math]::Min(20, $line108.Length - 52)))'"
    } else {
        Write-Host "Line 108 has only $($line108.Length) characters"
    }
    
    # Show surrounding lines for context
    Write-Host "`nContext (lines 106-110):" -ForegroundColor Yellow
    for ($i = 105; $i -le 109; $i++) {
        if ($i -lt $content.Length) {
            $lineNum = $i + 1
            $prefix = if ($lineNum -eq 108) { ">>> " } else { "    " }
            Write-Host "$prefix$lineNum: $($content[$i])"
        }
    }
} else {
    Write-Host "File has only $($content.Length) lines!" -ForegroundColor Red
}