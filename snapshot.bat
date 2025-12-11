@echo off
:: Snapshot tool for NinjaTrader-Indicators
:: Creates a file 'folder_structure.txt' showing the full directory tree

echo Generating folder structure snapshot...
tree /F /A > folder_structure.txt
echo.
echo ✅ Folder structure saved to folder_structure.txt
echo.
pause
