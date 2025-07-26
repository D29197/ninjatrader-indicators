
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

//This namespace holds Indicators in this folder and is required. Do not change it. 
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
				AddPlot(Brushes.Gray, "Histogram");
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

			Values[0][0] = macdLine;
			Values[1][0] = signalLine;
			Values[2][0] = histValue;

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
		private MACDFull[] cacheMACDFull;
		public MACDFull MACDFull()
		{
			return MACDFull(Input);
		}

		public MACDFull MACDFull(ISeries<double> input)
		{
			if (cacheMACDFull != null)
				for (int idx = 0; idx < cacheMACDFull.Length; idx++)
					if (cacheMACDFull[idx] != null &&  cacheMACDFull[idx].EqualsInput(input))
						return cacheMACDFull[idx];
			return CacheIndicator<MACDFull>(new MACDFull(), input, ref cacheMACDFull);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MACDFull MACDFull()
		{
			return indicator.MACDFull(Input);
		}

		public Indicators.MACDFull MACDFull(ISeries<double> input )
		{
			return indicator.MACDFull(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MACDFull MACDFull()
		{
			return indicator.MACDFull(Input);
		}

		public Indicators.MACDFull MACDFull(ISeries<double> input )
		{
			return indicator.MACDFull(input);
		}
	}
}
#endregion
