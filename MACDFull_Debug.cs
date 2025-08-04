
#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

namespace NinjaTrader.NinjaScript.Indicators
{
    public class MACDFull_Debug : Indicator
    {
        private MACD macd;

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name="Fast", GroupName="Parameters", Order=0)]
        public int Fast { get; set; } = 12;

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name="Slow", GroupName="Parameters", Order=1)]
        public int Slow { get; set; } = 26;

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name="Smooth", GroupName="Parameters", Order=2)]
        public int Smooth { get; set; } = 9;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "DEBUG VERSION: Always-on plot and MACD output logging.";
                Name = "MACDFull_Debug";
                IsOverlay = false;
                DrawOnPricePanel = false;  // <- THIS LINE IS CRITICAL
                AddPlot(Brushes.Blue, "MACD");
                AddPlot(Brushes.Orange, "Signal");
                AddPlot(Brushes.Gray, "Histogram");
            }
            else if (State == State.Configure)
            {
                macd = MACD(Fast, Slow, Smooth);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Math.Max(Slow, Smooth) + 1)
                return;

            double macdLine = macd?.Default[0] ?? 0;
            double signalLine = macd?.Avg[0] ?? 0;
            double histValue = macdLine - signalLine;

            Print($">>> [MACDFull_Debug] Time: {Time[0]}, MACD: {macdLine:F4}, Signal: {signalLine:F4}, Hist: {histValue:F4}");

            // Force renderable values to confirm display
            Values[0][0] = macdLine != 0 ? macdLine : 1;
            Values[1][0] = signalLine != 0 ? signalLine : 0.5;
            Values[2][0] = histValue != 0 ? histValue : 1.5;

            PlotBrushes[2][0] = histValue >= 0 ? Brushes.SteelBlue : Brushes.IndianRed;
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> MACDLine => Values[0];

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> SignalLine => Values[1];

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> Histogram => Values[2];

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private MACDFull_Debug[] cacheMACDFull_Debug;
        public MACDFull_Debug MACDFull_Debug(int fast, int slow, int smooth)
        {
            return MACDFull_Debug(Input, fast, slow, smooth);
        }

        public MACDFull_Debug MACDFull_Debug(ISeries<double> input, int fast, int slow, int smooth)
        {
            if (cacheMACDFull_Debug != null)
                for (int idx = 0; idx < cacheMACDFull_Debug.Length; idx++)
                    if (cacheMACDFull_Debug[idx] != null &&
                        cacheMACDFull_Debug[idx].Fast == fast &&
                        cacheMACDFull_Debug[idx].Slow == slow &&
                        cacheMACDFull_Debug[idx].Smooth == smooth &&
                        cacheMACDFull_Debug[idx].EqualsInput(input))
                        return cacheMACDFull_Debug[idx];
            return CacheIndicator<MACDFull_Debug>(new MACDFull_Debug() { Fast = fast, Slow = slow, Smooth = smooth }, input, ref cacheMACDFull_Debug);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.MACDFull_Debug MACDFull_Debug(int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug(Input, fast, slow, smooth);
        }

        public Indicators.MACDFull_Debug MACDFull_Debug(ISeries<double> input, int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug(input, fast, slow, smooth);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.MACDFull_Debug MACDFull_Debug(int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug(Input, fast, slow, smooth);
        }

        public Indicators.MACDFull_Debug MACDFull_Debug(ISeries<double> input, int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug(input, fast, slow, smooth);
        }
    }
}

#endregion
