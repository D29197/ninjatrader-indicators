//
// Copyright (C) 2025, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// The Volume Oscillator Range measures volume by calculating the difference of a fast and
	/// a slow moving average of volume. This custom copy is separated from the original
	/// Volume Oscillator so it can be modified independently.
	/// </summary>
	public class VolumeOscillatorRange : Indicator
	{
		private SMA smaFast;
		private SMA smaSlow;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= "Custom copy of Volume Oscillator.";
				Name						= "Volume Oscillator Range";
				IsSuspendedWhileInactive	= true;
				IsOverlay					= false;
				DrawOnPricePanel			= false;
				Fast						= 12;
				Slow						= 26;

				AddPlot(new Stroke(Brushes.DodgerBlue, 2), PlotStyle.Bar, "Volume Oscillator Range");
			}
			else if (State == State.DataLoaded)
			{
				smaFast	= SMA(Volume, Fast);
				smaSlow	= SMA(Volume, Slow);
			}
			else if (State == State.Historical)
			{
				if (Calculate == Calculate.OnPriceChange)
				{
					Draw.TextFixed(this, "NinjaScriptInfo", string.Format(Custom.Resource.NinjaScriptOnPriceChangeError, Name), TextPosition.BottomRight);
					Log(string.Format(Custom.Resource.NinjaScriptOnPriceChangeError, Name), LogLevel.Error);
				}
			}
		}

		protected override void OnBarUpdate()
		{
			double value = smaFast[0] - smaSlow[0];
			if (Instrument.MasterInstrument.InstrumentType == InstrumentType.CryptoCurrency)
				value = Core.Globals.ToCryptocurrencyVolume((long)value);
			Value[0] = value;
		}

		#region Properties
		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Fast", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Fast { get; set; }

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Slow", GroupName = "NinjaScriptParameters", Order = 1)]
		public int Slow { get; set; }
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private VolumeOscillatorRange[] cacheVolumeOscillatorRange;

		public VolumeOscillatorRange VolumeOscillatorRange(int fast, int slow)
		{
			return VolumeOscillatorRange(Input, fast, slow);
		}

		public VolumeOscillatorRange VolumeOscillatorRange(ISeries<double> input, int fast, int slow)
		{
			if (cacheVolumeOscillatorRange != null)
				for (int idx = 0; idx < cacheVolumeOscillatorRange.Length; idx++)
					if (cacheVolumeOscillatorRange[idx] != null
						&& cacheVolumeOscillatorRange[idx].Fast == fast
						&& cacheVolumeOscillatorRange[idx].Slow == slow
						&& cacheVolumeOscillatorRange[idx].EqualsInput(input))
						return cacheVolumeOscillatorRange[idx];

			return CacheIndicator<VolumeOscillatorRange>(new VolumeOscillatorRange() { Fast = fast, Slow = slow }, input, ref cacheVolumeOscillatorRange);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.VolumeOscillatorRange VolumeOscillatorRange(int fast, int slow)
		{
			return indicator.VolumeOscillatorRange(Input, fast, slow);
		}

		public Indicators.VolumeOscillatorRange VolumeOscillatorRange(ISeries<double> input, int fast, int slow)
		{
			return indicator.VolumeOscillatorRange(input, fast, slow);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.VolumeOscillatorRange VolumeOscillatorRange(int fast, int slow)
		{
			return indicator.VolumeOscillatorRange(Input, fast, slow);
		}

		public Indicators.VolumeOscillatorRange VolumeOscillatorRange(ISeries<double> input, int fast, int slow)
		{
			return indicator.VolumeOscillatorRange(input, fast, slow);
		}
	}
}

#endregion