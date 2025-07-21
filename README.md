# NinjaTrader Indicators Collection

This repository contains a set of custom NinjaTrader indicators developed to enhance trading analysis. These indicators are written in NinjaScript and are designed for manual import into NinjaTrader (version 8.1.5.2 or later).

## 📦 Included Indicators
- **EMALowMarker** – Draws a horizontal line at the lowest EMA value over a lookback period.
- **EMAFlatSegmentMarker** – Marks short flat segments when the EMA remains unchanged over consecutive bars.
- **EnhancedMACD_Full** – A modified MACD with histogram bars and colored signals plus MACD/Signal line overlays.

## 🔧 Installation Instructions (Manual Method)
1. Open NinjaTrader
2. Go to `New > NinjaScript Editor`
3. Expand the `Indicators` folder
4. Right-click on `Indicators` and select `Add New Indicator`
5. Enter the exact name (e.g., `EMALowMarker`), skip the description
6. On each wizard screen, leave default values (unless otherwise directed)
7. Once the generated template code appears, replace it fully with the content of the corresponding `.cs` file
8. Press `F5` or click Compile

## 🗂️ Structure
- Root folder contains the `.cs` files and `README.md`
- `/docs/` contains documentation and installation guides
