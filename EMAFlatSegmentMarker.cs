// EMAFlatSegmentMarker.cs – Marks flat EMA values
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
    public class EMAFlatSegmentMarker : Indicator
    {
        private Series<double> emaSeries;

        [NinjaScriptProperty]
        [Range(1, int.MaxValue), Display(Name = "Period", Order = 1)]
        public int Period { get; set; } = 14;

        [NinjaScriptProperty]
        [Range(0.00001, double.MaxValue), Display(Name = "Tolerance", Order = 2)]
        public double Tolerance { get; set; } = 0.0001;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Marks chart locations where the EMA remains flat within a defined tolerance.";
                Name = "EMAFlatSegmentMarker";
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
            if (CurrentBar < 2)
                return;

            emaSeries[0] = EMA(Period)[0];

            double diff = Math.Abs(emaSeries[0] - emaSeries[1]);
            if (diff <= Tolerance)
            {
                Draw.Dot(this, "FlatEMA_" + CurrentBar, false, 0, emaSeries[0], Brushes.Orange);
            }
        }
    }
}
