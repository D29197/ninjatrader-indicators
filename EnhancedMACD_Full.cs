
// EnhancedMACD_Full.cs - Cleaned for NinjaTrader 8.1.5.2
using System;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using System.Windows.Media;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class EnhancedMACD_Full : Indicator
    {
        private MACD macd;
        private Series<double> histogram;

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
                Description = "Enhanced MACD with colored histogram and MACD/Signal lines.";
                Name = "EnhancedMACD_Full";
                IsOverlay = false;
                AddPlot(Brushes.Blue, "MACD");
                AddPlot(Brushes.Orange, "Signal");
            }
            else if (State == State.DataLoaded)
            {
                macd = MACD(Fast, Slow, Smooth);
                histogram = new Series<double>(this);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1)
                return;

            histogram[0] = macd.Avg[0] != 0 ? macd.Default[0] - macd.Avg[0] : 0;

            Brush color = histogram[0] >= 0 ? Brushes.SteelBlue : Brushes.IndianRed;
            Draw.Bar(this, "MACDHist" + CurrentBar, false, 0, 0, histogram[0], color);

            Values[0][0] = macd.Default[0]; // MACD Line
            Values[1][0] = macd.Avg[0];     // Signal Line
        }
    }
}
