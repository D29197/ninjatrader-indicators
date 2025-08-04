
# NinjaTrader Custom Indicators

This repository contains three custom NinjaTrader indicators, designed to enhance chart analysis using MACD and EMA patterns.

## Indicators

### 1. MACDFull
- Plots MACD line, Signal line, and a color-coded histogram.
- Histogram color changes based on positive (green) or negative (red) divergence.
- Parameters: Fast, Slow, Smooth

### 2. EMALowMarker
- Draws a horizontal line at the lowest EMA(14) value over a customizable lookback period.
- Useful for identifying relative troughs in trend.

### 3. EMAFlatSegmentMarker
- Plots dots on the chart where the EMA(14) remains flat within a specified tolerance.
- Helps visualize sideways market movement or indecision.

## Installation

1. Open NinjaTrader.
2. Go to **NinjaScript Editor > Indicators > Add New**.
3. Replace the contents of the generated file with the code from the corresponding `.cs` file.
4. Compile and add the indicator to your chart from the **Indicators** window.

## Versioning

- Each `.cs` file includes internal version tags.
- Updates should be tracked using Git commits or GitHub releases for full repository versioning.

---

Maintained by: Darryl Conliffe

Current Version: 1.0.0
