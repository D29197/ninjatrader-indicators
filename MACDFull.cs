// MACDFull.cs – MACD with Signal and Color-coded Histogram
// Version: 1.0.0
using System;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using System.Windows.Media;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class MACDFull : Indicator
    {
        private MACD macd;

        [NinjaScriptProperty]
        public int Fast { get; set; } = 12;

        [NinjaScriptProperty]
        public int Slow { get; set; } = 26;

        [NinjaScriptProperty]
        public int Smooth { get; set; } = 9;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MACD with MACD/Signal and Color-coded Histogram";
                Name = "MACDFull";
                IsOverlay = false;
                AddPlot(Brushes.Blue, "MACD");
                AddPlot(Brushes.Orange, "Signal");
                AddPlot(Brushes.Gray, "Histogram"); // third plot
            }
            else if (State == State.DataLoaded)
            {
                macd = MACD(Fast, Slow, Smooth);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1)
                return;

            double macdLine = macd.Default[0];
            double signalLine = macd.Avg[0];
            double histValue = macdLine - signalLine;

            Values[0][0] = macdLine;     // MACD Line
            Values[1][0] = signalLine;   // Signal Line
            Values[2][0] = histValue;    // Histogram

            PlotBrushes[2][0] = histValue >= 0 ? Brushes.SteelBlue : Brushes.IndianRed;
        }
    }
}
