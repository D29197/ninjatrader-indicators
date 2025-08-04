// EMAFlatSegmentMarker.cs – Marks flat EMA values
// Version: 1.0.0
using System;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using System.Windows.Media;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class EMAFlatSegmentMarker : Indicator
    {
        private Series<double> emaSeries;

        [NinjaScriptProperty]
        public int Period { get; set; } = 14;

        [NinjaScriptProperty]
        public double Tolerance { get; set; } = 0.0001;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Draws segment where EMA is flat within a tolerance.";
                Name = "EMAFlatSegmentMarker";
                IsOverlay = true;
            }
            else if (State == State.DataLoaded)
            {
                emaSeries = EMA(Period);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 2)
                return;

            if (Math.Abs(emaSeries[0] - emaSeries[1]) < Tolerance)
            {
                Draw.Line(this, "Flat_" + CurrentBar, false, 1, emaSeries[1], 0, emaSeries[0], Brushes.Magenta, DashStyleHelper.Solid, 2);
            }
        }
    }
}
