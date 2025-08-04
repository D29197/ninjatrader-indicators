# NinjaTrader Indicators Repository

This repository contains custom NinjaScript indicators for use within the NinjaTrader 8.1.5.2 platform.

## Included Indicators

- **EMALowMarker.cs**: Draws a horizontal line at the lowest EMA over a lookback period.
- **EMAFlatSegmentMarker.cs**: Highlights flat segments of the EMA on the chart.
- **MACDFull.cs**: A custom MACD indicator with dual-line plots and a color-coded histogram.

## Manual Installation

To manually install these indicators in NinjaTrader:

1. Open NinjaTrader and navigate to: `New > NinjaScript Editor > Indicators`.
2. Right-click the `Indicators` folder and select **Add New Indicator**.
3. Follow the wizard:
   - Click **Next** through the welcome panel.
   - Enter the name (e.g., `EMALowMarker`), skipping the description since it's embedded in the code.
   - Leave default properties as-is unless instructed.
   - Click **Generate** to create the stub.
4. Replace the generated code block within the `NinjaTrader.NinjaScript.Indicators` namespace with the full content from the `.cs` file.
5. Press **F5** or click **Compile**.

## Versioning Strategy

This repository uses a **dual versioning approach**:

- **File-level versions**: Each `.cs` file includes a version tag comment near the top (e.g., `// Version: 1.2.0`).
- **System-level versioning**: Git is used to tag overall releases (e.g., `v1.2.0`).

Since this project has a single contributor and infrequent updates, versioning details are consolidated here in the `README.md`.

## Documentation

This repository includes a supplementary document containing coding conventions, known error patterns, troubleshooting tips, and style guidance for NinjaScript development:

- [NinjaTrader_Coding_Best_Practices_and_Error_Reference_v1.0.md](NinjaTrader_Coding_Best_Practices_and_Error_Reference_v1.0.md)

## Contact

This repository is maintained by Darryl Conliffe.
