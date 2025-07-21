
// EMAFlatSegmentMarker.cs - Cleaned for NinjaTrader 8.1.5.2
using System;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using System.Windows.Media;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class EMAFlatSegmentMarker : Indicator
    {
        private EMA ema;

        [NinjaScriptProperty]
        public int EmaPeriod { get; set; } = 14;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Draws horizontal segments when EMA is flat.";
                Name = "EMAFlatSegmentMarker";
                IsOverlay = true;
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 2)
                return;

            if (ema[0] == ema[1] && ema[1] == ema[2])
            {
                Draw.Line(this, "FlatSeg" + CurrentBar, false, 2, ema[2], 0, ema[0], Brushes.MediumVioletRed);
            }
        }
    }
}
