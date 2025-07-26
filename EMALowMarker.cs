
// EMALowMarker.cs - Cleaned for NinjaTrader 8.1.5.2
// Version: 1.0.0
using System;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using System.Windows.Media;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class EMALowMarker : Indicator
    {
        private EMA ema;
        private double lowestValue;

        [NinjaScriptProperty]
        public int EmaPeriod { get; set; } = 14;

        [NinjaScriptProperty]
        public int Lookback { get; set; } = 20;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Draws a horizontal line at the lowest EMA value in a lookback window.";
                Name = "EMALowMarker";
                IsOverlay = true;
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Lookback)
                return;

            lowestValue = double.MaxValue;
            for (int i = 0; i < Lookback; i++)
                lowestValue = Math.Min(lowestValue, ema[i]);

            Draw.HorizontalLine(this, "LowEMALine" + CurrentBar, lowestValue, Brushes.DodgerBlue);
        }
    }
}
