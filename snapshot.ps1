# Snapshot tool for NinjaTrader-Indicators
# Creates a timestamped folder structure listing

$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$outputFile = "folder_structure_$timestamp.txt"

Write-Host "📁 Generating folder structure snapshot..." -ForegroundColor Cyan
tree /F /A | Out-File -Encoding utf8 $outputFile
Write-Host ""
Write-Host "✅ Folder structure saved to $outputFile" -ForegroundColor Green
Write-Host ""
