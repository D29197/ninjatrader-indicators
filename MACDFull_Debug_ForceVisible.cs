
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
    public class MACDFull_Debug_ForceVisible : Indicator
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
                Description = "DEBUG VERSION with forced visibility: histogram bars, colored plots, wide strokes.";
                Name = "MACDFull_Debug_ForceVisible";
                IsOverlay = false;
                DrawOnPricePanel = false;
                DisplayInDataBox = true;
                PaintPriceMarkers = true;

                AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line, "MACD");
                AddPlot(new Stroke(Brushes.Orange, 2), PlotStyle.Line, "Signal");
                AddPlot(new Stroke(Brushes.Fuchsia, 4), PlotStyle.Bar, "Histogram");
            }
            else if (State == State.Configure)
            {
                macd = MACD(Fast, Slow, Smooth);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Math.Max(Slow, Smooth) + 1 || macd == null)
                return;

            double macdLine = macd.Default[0];
            double signalLine = macd.Avg[0];
            double histValue = macdLine - signalLine;

            Print($">>> [MACDFull_Debug_ForceVisible] Time: {Time[0]}, MACD: {macdLine:F4}, Signal: {signalLine:F4}, Hist: {histValue:F4}");

            if (double.IsNaN(histValue))
                histValue = 0.001;

            Values[0][0] = macdLine;
            Values[1][0] = signalLine;
            Values[2][0] = histValue;

            PlotBrushes[2][0] = histValue >= 0 ? Brushes.Fuchsia : Brushes.Yellow;
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
        private MACDFull_Debug_ForceVisible[] cacheMACDFull_Debug_ForceVisible;
        public MACDFull_Debug_ForceVisible MACDFull_Debug_ForceVisible(int fast, int slow, int smooth)
        {
            return MACDFull_Debug_ForceVisible(Input, fast, slow, smooth);
        }

        public MACDFull_Debug_ForceVisible MACDFull_Debug_ForceVisible(ISeries<double> input, int fast, int slow, int smooth)
        {
            if (cacheMACDFull_Debug_ForceVisible != null)
                for (int idx = 0; idx < cacheMACDFull_Debug_ForceVisible.Length; idx++)
                    if (cacheMACDFull_Debug_ForceVisible[idx] != null &&
                        cacheMACDFull_Debug_ForceVisible[idx].Fast == fast &&
                        cacheMACDFull_Debug_ForceVisible[idx].Slow == slow &&
                        cacheMACDFull_Debug_ForceVisible[idx].Smooth == smooth &&
                        cacheMACDFull_Debug_ForceVisible[idx].EqualsInput(input))
                        return cacheMACDFull_Debug_ForceVisible[idx];
            return CacheIndicator<MACDFull_Debug_ForceVisible>(new MACDFull_Debug_ForceVisible() { Fast = fast, Slow = slow, Smooth = smooth }, input, ref cacheMACDFull_Debug_ForceVisible);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.MACDFull_Debug_ForceVisible MACDFull_Debug_ForceVisible(int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug_ForceVisible(Input, fast, slow, smooth);
        }

        public Indicators.MACDFull_Debug_ForceVisible MACDFull_Debug_ForceVisible(ISeries<double> input, int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug_ForceVisible(input, fast, slow, smooth);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.MACDFull_Debug_ForceVisible MACDFull_Debug_ForceVisible(int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug_ForceVisible(Input, fast, slow, smooth);
        }

        public Indicators.MACDFull_Debug_ForceVisible MACDFull_Debug_ForceVisible(ISeries<double> input, int fast, int slow, int smooth)
        {
            return indicator.MACDFull_Debug_ForceVisible(input, fast, slow, smooth);
        }
    }
}
#endregion
