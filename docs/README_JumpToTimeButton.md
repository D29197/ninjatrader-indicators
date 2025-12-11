# JumpToTimeButton (NinjaTrader 8.1.x)

A chart toolbar add-on that lets you **jump and center** the viewport on a specific time for any **loaded** date/session. It includes a dialog, one-click **presets** (09:30, 12:00, 15:59), and **Prev/Next Day** anchors.

## Features

- **Jump → Time** dialog accepts:
  - `HH:mm` → anchors to the **left edge** of the currently visible day
  - `yyyy-MM-dd HH:mm` → jumps to exact date/time and resets the preset anchor to that day
- **Presets:** 09:30, 12:00, 15:59 (anchor to the current visible day)
- **Prev/Next Day:** shifts the preset anchor ±1 day
- **AutoZoomIfNeeded:** if the chart is fully zoomed out (no horizontal scroll range), it gently increases **BarSpacing** to create just enough range so it can **center** on the target time
- **DebugMode:** logs steps to **New → NinjaScript Output** for transparency

## How it works (design considerations)

1. **Loaded-data bound:** NinjaTrader only lets you navigate within data currently loaded in the chart (respecting *Days to load* and your *Trading Hours* template).
2. **Find the bar:** Input time is resolved to the nearest **loaded** bar using `Bars.GetBar()`. If outside the loaded range, the indicator tells you the range and exits.
3. **Visual marker:** A vertical line is drawn at the resolved bar for confirmation.
4. **Centering:** Because NinjaTrader doesn’t expose a public “set visible range” API, the indicator pans by adjusting the chart’s **horizontal scrollbar** so the target bar sits mid-viewport.
5. **Auto-zoom fallback:** If there’s no scroll range (fully zoomed out), and `AutoZoomIfNeeded = true`, it gently **increases BarSpacing** a few steps to create horizontal range, then centers again.
6. **Anchor logic:**  
   - `HH:mm` uses the **left edge of the visible chart** as the anchor date, so you can scroll to any day and fire presets without retyping a date.
   - Typing a full timestamp (`yyyy-MM-dd HH:mm`) jumps there and updates the internal anchor so presets follow that date.

## Parameters

- **DebugMode** (bool, default `true`): Print breadcrumbs to NinjaScript Output.
- **AutoZoomIfNeeded** (bool, default `true`): Create scroll range automatically when none exists.
- **AutoZoomMaxSteps** (int, default `10`): Safety cap on spacing increments during auto-zoom.

## Usage

1. **Install:** Add the indicator to a chart (Indicators → JumpToTimeButton).
2. **Open Output:** View logs via Control Center → **New → NinjaScript Output**.
3. **Jump:** Click **Jump → Time**, enter `HH:mm` or `yyyy-MM-dd HH:mm`.
4. **Presets:** Use 09:30 / 12:00 / 15:59 to snap to common times on the currently visible day.
5. **Prev/Next:** Shift the preset day quickly without typing.

## Notes & limitations

- You can only jump to times that are **actually loaded**. If you need older days or overnight sessions, increase **Days to load** and check your **Trading Hours**.
- On some builds/themes, programmatic bar-spacing changes may not be available; in that case Auto-zoom will simply do nothing and you’ll see a friendly message. You can manually zoom in (mouse wheel) and try again.
- This add-on avoids unsupported internal API calls; it uses **public** UI elements (WPF controls and the scrollbar) in a guarded, best-effort manner.

## Troubleshooting

- **“Requested time is outside the loaded range”**: Increase *Days to load* or adjust *Trading Hours*.
- **Viewport doesn’t move / no scroll range**: Enable `AutoZoomIfNeeded` or zoom in slightly; verify scroll logs in NinjaScript Output.
- **No buttons visible**: Remove and re-add the indicator; ensure you’re on NinjaTrader 8.1.x.

## Versioning

- v1.1.0 — Presets + Prev/Next + AutoZoomIfNeeded + centered confirmation
- v1.0.0 — Initial Jump → Time + vertical marker + best-effort centering
