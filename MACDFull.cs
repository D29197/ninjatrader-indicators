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

