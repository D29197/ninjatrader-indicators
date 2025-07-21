
README - Cleaned NinjaTrader Indicators (v8.1.5.2+)

Included Files:
1. EMALowMarker.cs - Draws a horizontal line at the lowest EMA value over a lookback period.
2. EMAFlatSegmentMarker.cs - Draws horizontal segments when the EMA remains flat for 3 bars.
3. EnhancedMACD_Full.cs - Combines MACD histogram with standard MACD and Signal line plots.

Installation Notes:
These scripts must be added manually. You cannot import them using the NinjaScript ZIP import tool.

Correct Manual Installation (as of NinjaTrader 8.1.5.2):
1. Open NinjaTrader.
2. Go to: New > NinjaScript Editor.
3. In the editor, expand the 'Indicators' folder.
4. Right-click 'Indicators' and select 'Add New Indicator'.
5. Walk through the panels:
   - **Panel 1**: Click Next.
   - **Panel 2**: Enter name (e.g., EMALowMarker). Skip description — it's in the code.
   - **Panel 3**: Leave 'Calculate on bar close' checked.
                 For EMALowMarker and EMAFlatSegmentMarker, CHECK 'Overlay on price'.
                 For EnhancedMACD_Full, LEAVE 'Overlay on price' UNCHECKED.
   - **Panel 4**: Leave all options unchecked (no additional series or data inputs).
6. Click 'Generate'.
7. In the editor window that opens, DELETE all code and paste the entire content from the matching .cs file (including `using` lines).
8. Press F5 or click Compile.

Prepared by ChatGPT
July 2025
