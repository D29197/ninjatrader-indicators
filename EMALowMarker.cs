// EMALowMarker.cs – Draws horizontal line at lowest EMA
// Version: 1.1.0
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

namespace NinjaTrader.NinjaScript.Indicators
{
    public class EMALowMarker : Indicator
    {
        private Series<double> emaSeries;
        private double lowestEma;

        [NinjaScriptProperty]
        [Range(1, int.MaxValue), Display(Name = "Period", Order = 1)]
        public int Period { get; set; } = 14;

        [NinjaScriptProperty]
        [Range(1, int.MaxValue), Display(Name = "Lookback", Order = 2)]
        public int Lookback { get; set; } = 50;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Draws a horizontal line at the lowest EMA over a lookback period.";
                Name = "EMALowMarker";
                IsOverlay = true;
                Calculate = Calculate.OnBarClose;
                IsSuspendedWhileInactive = true;
            }
            else if (State == State.DataLoaded)
            {
                emaSeries = new Series<double>(this);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Lookback)
                return;

            emaSeries[0] = EMA(Period)[0];

            lowestEma = double.MaxValue;
            for (int i = 0; i < Lookback; i++)
                if (emaSeries[i] < lowestEma)
                    lowestEma = emaSeries[i];

            Draw.HorizontalLine(this, "LowEMA_" + CurrentBar, lowestEma, Brushes.SteelBlue);
        }
    }
}
