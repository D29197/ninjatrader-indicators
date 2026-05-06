#region Using declarations
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using SharpDX;
#endregion

// ----------------------------------------------------------------------
//  ENUM DEFINITION (outside namespace for global visibility)
// ----------------------------------------------------------------------
public enum StopMarkerDisplayValue
{
    TICKS,
    CURRENCY,
    BOTH
}

namespace NinjaTrader.NinjaScript.Indicators
{
    public class StopMarker : Indicator
    {
        protected enum StopMarkerOrderType { STOP, TARGET }

        class OrderTypeAndText
        {
            public StopMarkerOrderType orderType;
            public string text;
        }

        private Account gAccount;
        private AccountSelector gAccountSelector;
        private Dictionary<string, int> orderQtyTracker;
        private Dictionary<double, OrderTypeAndText> toRender;
        private SharpDX.DirectWrite.TextFormat textFormat;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Displays Stop/Target information beside order lines.";
                Name = "StopMarker";
                Calculate = Calculate.OnEachTick;
                IsOverlay = true;
                DrawOnPricePanel = true;
                IsSuspendedWhileInactive = true;

                Font = new SimpleFont("Arial", 12);
                DisplayMode = StopMarkerDisplayValue.BOTH;

                StopFillBrush = Brushes.Maroon;
                TargetFillBrush = Brushes.DarkGreen;
                OutlineBrush = Brushes.AliceBlue;
                TextBrush = Brushes.White;

                LeftOffset = 250f;
                VerticalOffset = 0f;
            }
            else if (State == State.Configure)
            {
                orderQtyTracker = new Dictionary<string, int>();
                toRender = new Dictionary<double, OrderTypeAndText>();
            }
            else if (State == State.DataLoaded)
            {
                textFormat = Font.ToDirectWriteTextFormat();
            }
        }

        protected override void OnBarUpdate()
        {
            if (State == State.Historical) return;

            ChartControl.Dispatcher.InvokeAsync((Action)(() =>
            {
                gAccountSelector = Window.GetWindow(ChartControl.Parent)
                    .FindFirst("ChartTraderControlAccountSelector") as AccountSelector;
                gAccount = gAccountSelector?.SelectedAccount;
            }));

            if (gAccount == null) return;

            orderQtyTracker.Clear();
            toRender.Clear();

            foreach (Position p in gAccount.Positions)
            {
                if (p.Instrument != Instrument || p.MarketPosition == MarketPosition.Flat)
                    continue;

                double entryPrice = p.AveragePrice;

                foreach (Order order in gAccount.Orders)
                {
                    if (order.Instrument != Instrument) continue;
                    if (!(order.OrderState == OrderState.Accepted || order.OrderState == OrderState.Working))
                        continue;
                    if ((p.MarketPosition == MarketPosition.Long && order.IsLong) ||
                        (p.MarketPosition == MarketPosition.Short && order.IsShort))
                        continue;

                    double orderPrice = GetOrderPrice(order);
                    if (orderPrice == 0) continue;

                    string key = orderPrice + order.OrderType.ToString();
                    if (orderQtyTracker.ContainsKey(key))
                        orderQtyTracker[key] += order.Quantity;
                    else
                        orderQtyTracker[key] = order.Quantity;

                    int orderQty = orderQtyTracker[key];
                    double priceDiff = (p.MarketPosition == MarketPosition.Long ? orderPrice - entryPrice : entryPrice - orderPrice);
                    int ticks = (int)(priceDiff / TickSize);
                    double currencyValue = priceDiff * Instrument.MasterInstrument.PointValue * orderQty;

                    string orderType = order.IsStopLimit || order.IsStopMarket ? "STOP" : "TARGET";
                    string text =
                        orderType + " (" + orderQty + ") " +
                        ((DisplayMode == StopMarkerDisplayValue.TICKS || DisplayMode == StopMarkerDisplayValue.BOTH
                            ? ((order.IsStopLimit || order.IsStopMarket) && ticks > 0 ? "+" : "") + ticks + " ticks"
                            : "")
                        + (DisplayMode == StopMarkerDisplayValue.BOTH ? " : " : "")
                        + (DisplayMode == StopMarkerDisplayValue.CURRENCY || DisplayMode == StopMarkerDisplayValue.BOTH
                            ? currencyValue.ToString("C2") : ""));

                    toRender[orderPrice] = new OrderTypeAndText()
                    {
                        orderType = (order.IsStopLimit || order.IsStopMarket
                            ? StopMarkerOrderType.STOP
                            : StopMarkerOrderType.TARGET),
                        text = text
                    };
                }
            }

            ForceRefresh();
        }

        private double GetOrderPrice(Order order)
        {
            if (order.IsStopLimit || order.IsStopMarket || order.IsMarketIfTouched)
                return order.StopPrice;
            else if (order.IsLimit)
                return order.LimitPrice;
            return 0;
        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
            if (State == State.Historical || toRender == null || toRender.Count == 0) return;

            using (var borderBrushDx = OutlineBrush.ToDxBrush(RenderTarget))
            using (var stopBrushDx = StopFillBrush.ToDxBrush(RenderTarget))
            using (var targetBrushDx = TargetFillBrush.ToDxBrush(RenderTarget))
            using (var textBrushDx = TextBrush.ToDxBrush(RenderTarget))
            {
                foreach (var kvp in toRender)
                {
                    var textLayout = new SharpDX.DirectWrite.TextLayout(
                        NinjaTrader.Core.Globals.DirectWriteFactory,
                        kvp.Value.text, textFormat,
                        ChartPanel.W, textFormat.FontSize);

                    float textWidth = textLayout.Metrics.Width;
                    float textHeight = textLayout.Metrics.Height;

                    double barLengthPercent = 15;
                    try
                    {
                        if (ChartControl?.OwnerChart?.ChartTrader?.Properties != null)
                            barLengthPercent = ChartControl.OwnerChart.ChartTrader.Properties.OrderDisplayBarLength;
                    }
                    catch { }

                    float x = (float)(
                        ChartPanel.W
                        - (ChartPanel.W * (barLengthPercent / 100.0))
                        - textWidth
                        - LeftOffset);

                    int priceCoordinate = chartScale.GetYByValue(kvp.Key);
                    float y = priceCoordinate - ((textHeight + 7) / 2) - VerticalOffset;

                    var startPoint = new Vector2(x, y);
                    var upperTextPoint = new Vector2(startPoint.X + 4, startPoint.Y + 3);
                    var lineStartPoint = new Vector2(startPoint.X + textWidth + 9, priceCoordinate);
                    var lineEndPoint = new Vector2(ChartPanel.W, priceCoordinate);

                    var rect = new SharpDX.RectangleF(startPoint.X, startPoint.Y, textWidth + 8, textHeight + 6);
                    RenderTarget.FillRectangle(rect, kvp.Value.orderType == StopMarkerOrderType.STOP ? stopBrushDx : targetBrushDx);
                    RenderTarget.DrawRectangle(rect, borderBrushDx, 1);
                    RenderTarget.DrawLine(lineStartPoint, lineEndPoint, borderBrushDx);
                    RenderTarget.DrawTextLayout(upperTextPoint, textLayout, textBrushDx, SharpDX.Direct2D1.DrawTextOptions.NoSnap);

                    textLayout.Dispose();
                }
            }
        }

        #region Parameters
        [NinjaScriptProperty]
        [Display(Name = "Font", GroupName = "Parameters", Order = 100)]
        public SimpleFont Font { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Display", GroupName = "Parameters", Order = 200)]
        public StopMarkerDisplayValue DisplayMode { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Stop Fill Color", GroupName = "Colors", Order = 100)]
        public Brush StopFillBrush { get; set; }

        [Browsable(false)]
        public string StopFillBrushSerializable
        {
            get { return Serialize.BrushToString(StopFillBrush); }
            set { StopFillBrush = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Target Fill Color", GroupName = "Colors", Order = 200)]
        public Brush TargetFillBrush { get; set; }

        [Browsable(false)]
        public string TargetFillBrushSerializable
        {
            get { return Serialize.BrushToString(TargetFillBrush); }
            set { TargetFillBrush = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Outline Color", GroupName = "Colors", Order = 300)]
        public Brush OutlineBrush { get; set; }

        [Browsable(false)]
        public string OutlineBrushSerializable
        {
            get { return Serialize.BrushToString(OutlineBrush); }
            set { OutlineBrush = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Text Color", GroupName = "Colors", Order = 400)]
        public Brush TextBrush { get; set; }

        [Browsable(false)]
        public string TextBrushSerializable
        {
            get { return Serialize.BrushToString(TextBrush); }
            set { TextBrush = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Range(0, 600)]
        [Display(Name = "Left Offset", GroupName = "Visual", Order = 500)]
        public float LeftOffset { get; set; }

        [NinjaScriptProperty]
        [Range(0, 50)]
        [Display(Name = "Vertical Offset", GroupName = "Visual", Order = 600)]
        public float VerticalOffset { get; set; }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private StopMarker[] cacheStopMarker;

        public StopMarker StopMarker(SimpleFont font,
            StopMarkerDisplayValue displayMode,
            Brush stopFillBrush, Brush targetFillBrush,
            Brush outlineBrush, Brush textBrush,
            float leftOffset, float verticalOffset)
        {
            return StopMarker(Input, font, displayMode,
                stopFillBrush, targetFillBrush, outlineBrush, textBrush,
                leftOffset, verticalOffset);
        }

        public StopMarker StopMarker(ISeries<double> input,
            SimpleFont font,
            StopMarkerDisplayValue displayMode,
            Brush stopFillBrush, Brush targetFillBrush,
            Brush outlineBrush, Brush textBrush,
            float leftOffset, float verticalOffset)
        {
            if (cacheStopMarker != null)
                for (int idx = 0; idx < cacheStopMarker.Length; idx++)
                    if (cacheStopMarker[idx] != null &&
                        cacheStopMarker[idx].Font == font &&
                        cacheStopMarker[idx].DisplayMode == displayMode &&
                        cacheStopMarker[idx].StopFillBrush == stopFillBrush &&
                        cacheStopMarker[idx].TargetFillBrush == targetFillBrush &&
                        cacheStopMarker[idx].OutlineBrush == outlineBrush &&
                        cacheStopMarker[idx].TextBrush == textBrush &&
                        cacheStopMarker[idx].LeftOffset == leftOffset &&
                        cacheStopMarker[idx].VerticalOffset == verticalOffset &&
                        cacheStopMarker[idx].EqualsInput(input))
                        return cacheStopMarker[idx];

            return CacheIndicator<StopMarker>(
                new StopMarker()
                {
                    Font = font,
                    DisplayMode = displayMode,
                    StopFillBrush = stopFillBrush,
                    TargetFillBrush = targetFillBrush,
                    OutlineBrush = outlineBrush,
                    TextBrush = textBrush,
                    LeftOffset = leftOffset,
                    VerticalOffset = verticalOffset
                },
                input, ref cacheStopMarker);
        }
    }
}
#endregion
