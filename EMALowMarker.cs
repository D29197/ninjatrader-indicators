#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
    public class EMALowMarker : Indicator
    {
        private EMA ema;

        [NinjaScriptProperty]
        [Range(1, int.MaxValue), Display(Name = "EMA Period", Description = "EMA period", Order = 1, GroupName = "Parameters")]
        public int EmaPeriod { get; set; } = 14;

        [NinjaScriptProperty]
        [Range(1, int.MaxValue), Display(Name = "Lookback Period", Description = "Lookback period for lowest value", Order = 2, GroupName = "Parameters")]
        public int LookbackPeriod { get; set; } = 50;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Draws a horizontal line at the lowest EMA value over a lookback period.";
                Name = "EMALowMarker";
                IsOverlay = true;
                Calculate = Calculate.OnBarClose;
                IsSuspendedWhileInactive = true;
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < LookbackPeriod)
                return;

            double lowestEma = double.MaxValue;
            for (int i = 0; i < LookbackPeriod; i++)
            {
                double val = ema[i];
                if (val < lowestEma)
                    lowestEma = val;
            }

            Draw.HorizontalLine(this, "EMALowLine" + CurrentBar, false, lowestEma, Brushes.DarkGreen);
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private EMALowMarker[] cacheEMALowMarker;
        public EMALowMarker EMALowMarker(int emaPeriod, int lookbackPeriod)
        {
            return EMALowMarker(Input, emaPeriod, lookbackPeriod);
        }

        public EMALowMarker EMALowMarker(ISeries<double> input, int emaPeriod, int lookbackPeriod)
        {
            if (cacheEMALowMarker != null)
                for (int idx = 0; idx < cacheEMALowMarker.Length; idx++)
                    if (cacheEMALowMarker[idx] != null && cacheEMALowMarker[idx].EmaPeriod == emaPeriod && cacheEMALowMarker[idx].LookbackPeriod == lookbackPeriod && cacheEMALowMarker[idx].EqualsInput(input))
                        return cacheEMALowMarker[idx];
            return CacheIndicator<EMALowMarker>(new EMALowMarker() { EmaPeriod = emaPeriod, LookbackPeriod = lookbackPeriod }, input, ref cacheEMALowMarker);
        }
    }
}

#endregion
