#region Using declarations
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;                  // DisplayName, Category, Description
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

// NinjaTrader 8.1.x
// Toolbar helpers to JUMP the chart to a specific date/time (within loaded data).
// - "Jump → Time" accepts "HH:mm" (anchored to the left-edge visible date) or "yyyy-MM-dd HH:mm".
// - Presets: 09:30, 12:00, 15:59 (anchored to visible left-edge date, or to override date after Prev/Next).
// - Prev Day / Next Day shifts the preset anchor date by ±1 day.
// Behavior:
// 1) Resolves to nearest loaded bar; draws a vertical marker.
// 2) Centers the viewport by adjusting the chart's horizontal scrollbar.
// 3) If no horizontal scroll range and AutoZoomIfNeeded is true, gently increases BarSpacing (via reflection if available) and then centers.
// Notes:
// • You can only jump within LOADED data (Days to load + Trading Hours).
// • Open Control Center → New → NinjaScript Output to view DebugMode logs.

namespace NinjaTrader.NinjaScript.Indicators
{
    public class JumpToTimeButton : Indicator
    {
        private Chart       chartWindow;
        private StackPanel  buttonStrip;
        private Button      btnJump, btn0930, btn1200, btn1559, btnPrev, btnNext;
        private bool        added;
        private DateTime?   overrideAnchorDate; // set by Prev/Next or full timestamp jump

        #region User inputs
        [NinjaScriptProperty]
        [DisplayName("DebugMode")]
        [Description("Print debug messages to NinjaScript Output")]
        [Category("Parameters")]
        public bool DebugMode { get; set; } = true;

        [NinjaScriptProperty]
        [DisplayName("AutoZoomIfNeeded")]
        [Description("If no horizontal scroll range, gently increase bar spacing so the chart can pan")]
        [Category("Parameters")]
        public bool AutoZoomIfNeeded { get; set; } = true;

        [NinjaScriptProperty]
        [DisplayName("AutoZoomMaxSteps")]
        [Description("Maximum bar-spacing increments when auto-zooming (safety cap)")]
        [Category("Parameters")]
        public int AutoZoomMaxSteps { get; set; } = 10;
        #endregion

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Name        = "JumpToTimeButton";
                Description = "Toolbar buttons to jump chart to a specific time (within loaded data).";
                IsOverlay   = true;
                Calculate   = Calculate.OnBarClose;
                DebugMode   = true;
                AutoZoomIfNeeded = true;
                AutoZoomMaxSteps = 10;
            }
            else if (State == State.Historical)
            {
                TryAddButtons();
            }
            else if (State == State.Terminated)
            {
                TryRemoveButtons();
            }
        }

        protected override void OnBarUpdate() { /* no per-bar calc */ }

        // ----------------- UI wiring -----------------
        private void TryAddButtons()
        {
            if (ChartControl == null || added) return;

            ChartControl.Dispatcher.InvokeAsync(() =>
            {
                chartWindow = Window.GetWindow(ChartControl.Parent) as Chart;
                if (chartWindow == null)
                {
                    DPrint("TryAddButtons: chartWindow is null.");
                    return;
                }

                if (buttonStrip == null)
                {
                    buttonStrip = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(4, 0, 0, 0)
                    };

                    btnJump  = MakeBtn("Jump → Time", OnJumpClick, "Enter HH:mm or YYYY-MM-DD HH:mm");
                    btnPrev  = MakeBtn("⟨ Prev Day", (_, __) => ShiftAnchor(-1), "Anchor presets to previous day");
                    btnNext  = MakeBtn("Next Day ⟩",  (_, __) => ShiftAnchor(1),  "Anchor presets to next day");
                    btn0930  = MakeBtn("09:30", (_, __) => JumpPreset(9, 30));
                    btn1200  = MakeBtn("12:00", (_, __) => JumpPreset(12, 0));
                    btn1559  = MakeBtn("15:59", (_, __) => JumpPreset(15, 59));

                    buttonStrip.Children.Add(btnJump);
                    buttonStrip.Children.Add(Spacer());
                    buttonStrip.Children.Add(btnPrev);
                    buttonStrip.Children.Add(btnNext);
                    buttonStrip.Children.Add(Spacer());
                    buttonStrip.Children.Add(btn0930);
                    buttonStrip.Children.Add(btn1200);
                    buttonStrip.Children.Add(btn1559);
                }

                chartWindow.MainMenu.Add(buttonStrip);
                added = true;
                DPrint("TryAddButtons: buttons added.");
            });
        }

        private void TryRemoveButtons()
        {
            if (!added || chartWindow == null || buttonStrip == null) return;

            ChartControl?.Dispatcher.InvokeAsync(() =>
            {
                try { chartWindow.MainMenu.Remove(buttonStrip); } catch { /* ignore */ }
                btnJump = btn0930 = btn1200 = btn1559 = btnPrev = btnNext = null;
                buttonStrip = null;
                added = false;
                DPrint("TryRemoveButtons: buttons removed.");
            });
        }

        private Button MakeBtn(string content, RoutedEventHandler onClick, string tooltip = null)
        {
            var b = new Button
            {
                Content = content,
                Margin = new Thickness(2, 0, 2, 0),
                Padding = new Thickness(8, 2, 8, 2),
                FontSize = 11,
                ToolTip = tooltip
            };
            b.Click += onClick;
            return b;
        }
        private FrameworkElement Spacer() => new Separator { Width = 8, Opacity = 0 };

        private void ShiftAnchor(int days)
        {
            DateTime baseDate = overrideAnchorDate ?? GetLeftEdgeDate();
            overrideAnchorDate = baseDate.AddDays(days);
            DPrint($"ShiftAnchor: overrideAnchorDate={overrideAnchorDate:yyyy-MM-dd}");
        }

        private void JumpPreset(int hh, int mm)
        {
            DateTime anchor = overrideAnchorDate ?? GetLeftEdgeDate();
            DateTime requested = anchor.AddHours(hh).AddMinutes(mm);
            DPrint($"JumpPreset: {requested:yyyy-MM-dd HH:mm:ss}");
            JumpTo(requested);
        }

        // ----------------- Jump → Time dialog -----------------
        private void OnJumpClick(object sender, RoutedEventArgs e)
        {
            var prompt = new Window
            {
                Title = "Jump to Time",
                Width = 380,
                Height = 160,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(ChartControl?.Parent)
            };

            var grid = new Grid { Margin = new Thickness(12) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var lbl = new TextBlock { Text = "Enter HH:mm  (or YYYY-MM-DD HH:mm):", Margin = new Thickness(0, 0, 0, 6) };
            Grid.SetRow(lbl, 0);
            grid.Children.Add(lbl);

            var box = new TextBox { Text = DateTime.Now.ToString("HH:mm") };
            Grid.SetRow(box, 1);
            grid.Children.Add(box);

            var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
            var ok = new Button { Content = "OK", MinWidth = 70, Margin = new Thickness(0, 0, 8, 0) };
            var cancel = new Button { Content = "Cancel", MinWidth = 70 };
            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            Grid.SetRow(buttons, 2);
            grid.Children.Add(buttons);

            ok.Click += (_, __) => { prompt.DialogResult = true;  prompt.Close(); };
            cancel.Click += (_, __) => { prompt.DialogResult = false; prompt.Close(); };

            prompt.Content = grid;
            if (prompt.ShowDialog() != true)
            {
                DPrint("OnJumpClick: user canceled.");
                return;
            }

            var input = box.Text?.Trim() ?? string.Empty;
            DPrint($"OnJumpClick: raw input = \"{input}\"");

            DateTime requested;
            if (!TryParseTime(input, out requested))
            {
                MessageBox.Show("Invalid time. Use HH:mm or YYYY-MM-DD HH:mm", "Jump to Time");
                DPrint("OnJumpClick: parse failed.");
                return;
            }

            // HH:mm → anchor to the visible left-edge date (or override if prev/next used)
            if (input.Length <= 5)
            {
                DateTime baseDate = overrideAnchorDate ?? GetLeftEdgeDate();
                requested = baseDate.AddHours(requested.Hour).AddMinutes(requested.Minute);
                DPrint($"OnJumpClick: HH:mm only. BaseDate={baseDate:yyyy-MM-dd}, requested={requested:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                // Full timestamp → use as-is and make presets follow that day
                overrideAnchorDate = requested.Date;
                DPrint($"OnJumpClick: full timestamp. requested={requested:yyyy-MM-dd HH:mm:ss}");
            }

            JumpTo(requested);
        }

        // ----------------- Core logic -----------------
        private void JumpTo(DateTime requested)
        {
            DPrint($"JumpTo: start. Count={Count}, requested={requested:yyyy-MM-dd HH:mm:ss}");

            if (Bars == null || Count < 1)
            {
                MessageBox.Show("No data loaded on this chart.", "Jump to Time");
                DPrint("JumpTo: no Bars/Count < 1.");
                return;
            }

            DateTime latest = Times[0][0];               // most recent bar
            DateTime earliest = Times[0][Count - 1];     // oldest loaded bar
            DPrint($"JumpTo: loaded range = {earliest:yyyy-MM-dd HH:mm:ss} → {latest:yyyy-MM-dd HH:mm:ss}");

            if (requested < earliest || requested > latest)
            {
                MessageBox.Show(
                    $"Requested time is outside the loaded range.\n\n" +
                    $"Loaded: {earliest:yyyy-MM-dd HH:mm:ss} → {latest:yyyy-MM-dd HH:mm:ss}\n" +
                    $"Requested: {requested:yyyy-MM-dd HH:mm:ss}\n\n" +
                    $"Tip: increase 'Days to load' or adjust the Trading Hours template.",
                    "Jump to Time");
                DPrint("JumpTo: requested outside loaded range.");
                return;
            }

            // ---------- SAFE BAR LOOKUP ----------
            int rawIdx  = Bars.GetBar(requested);
            int current = Math.Max(0, CurrentBar);
            int barsAgo = rawIdx;

            // If result looks like absolute index or is out of range, convert to barsAgo
            int absToBarsAgo = current - rawIdx;
            if (barsAgo < 0 || barsAgo > current) barsAgo = absToBarsAgo;

            // Clamp inside [0 .. CurrentBar], and avoid exactly CurrentBar (oldest bar) to dodge edge errors
            if (barsAgo < 0) barsAgo = 0;
            if (barsAgo >= current && current > 0) barsAgo = current - 1;

            DPrint($"JumpTo: GetBar rawIdx={rawIdx}, CurrentBar={current}, resolved barsAgo={barsAgo}");

            DateTime barTime;
            try
            {
                barTime = Times[0][barsAgo];
            }
            catch (Exception ex)
            {
                // Ultra-defensive fallback: clamp again and retry
                barsAgo = Math.Max(0, Math.Min(Math.Max(0, CurrentBar - 1), barsAgo));
                DPrint($"JumpTo: Times[0][{barsAgo}] retry after '{ex.Message}'");
                barTime = Times[0][barsAgo];
            }
            DPrint($"JumpTo: resolved barTime={barTime:yyyy-MM-dd HH:mm:ss}");
            // ---------- /SAFE BAR LOOKUP ----------

            // Draw a visible marker
            string tag = "JTT_" + barTime.ToString("yyyyMMdd_HHmmss");
            Draw.VerticalLine(this, tag, barTime, Brushes.DodgerBlue);
            DPrint($"JumpTo: Draw.VerticalLine tag={tag}");

            // Try to pan viewport via horizontal ScrollBar
            ChartControl.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    double slot = ChartControl.GetSlotIndexByTime(barTime);
                    DPrint($"UI: slot (GetSlotIndexByTime) = {slot}");

                    ScrollBar hsb = FindHorizontalScrollBar(chartWindow);
                    if (hsb == null)
                    {
                        DPrint("UI: Horizontal ScrollBar NOT found.");
                        Print("JumpToTimeButton: Marker drawn. If the view didn’t move, zoom in (to create scroll range) or use right-click time axis → Go To…");
                        return;
                    }

                    if (!EnsureScrollable(hsb))
                    {
                        Print("JumpToTimeButton: View cannot pan (no horizontal scroll range). Zoom in slightly, then try again.");
                        return;
                    }

                    // Center the target
                    double viewport = hsb.ViewportSize > 0 ? hsb.ViewportSize : 100.0;
                    double target   = Math.Max(hsb.Minimum, Math.Min(hsb.Maximum, slot - viewport / 2.0));
                    double before   = hsb.Value;
                    hsb.Value = target;

                    DPrint($"UI: Centered. min={hsb.Minimum}, max={hsb.Maximum}, viewport={viewport}, before={before}, target={target}, after={hsb.Value}");
                    Print($"JumpToTimeButton: Centered on {barTime:yyyy-MM-dd HH:mm}.");
                }
                catch (Exception ex)
                {
                    DPrint("UI exception: " + ex.Message);
                }
            });
        }

        // Ensure there is horizontal scroll range; optionally auto-zoom via BarSpacing (reflection)
        private bool EnsureScrollable(ScrollBar hsb)
        {
            double range = hsb.Maximum - hsb.Minimum;
            if (range > double.Epsilon) return true;

            if (!AutoZoomIfNeeded)
            {
                DPrint("UI: No scroll range and AutoZoomIfNeeded=false.");
                return false;
            }

            // Try to gently increase BarSpacing via reflection (cross-build safe)
            try
            {
                var props = ChartControl?.Properties;
                if (props != null)
                {
                    var barSpacingProp = props.GetType().GetProperty("BarSpacing");
                    if (barSpacingProp != null)
                    {
                        for (int step = 0; step < Math.Max(1, AutoZoomMaxSteps); step++)
                        {
                            int before = (int)barSpacingProp.GetValue(props);
                            barSpacingProp.SetValue(props, before + 1);
                            DPrint($"UI: AutoZoom step {step + 1}: BarSpacing {before} -> {before + 1}");

                            range = hsb.Maximum - hsb.Minimum;
                            if (range > double.Epsilon)
                            {
                                DPrint($"UI: Scroll range created after {step + 1} step(s).");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        DPrint("UI: AutoZoom not supported (no BarSpacing property).");
                    }
                }
            }
            catch (Exception ex)
            {
                DPrint($"UI: AutoZoom failed ({ex.Message}).");
            }

            DPrint("UI: Could not create scroll range; view remains fixed.");
            return false;
        }

        // ----------------- Helpers -----------------
        private DateTime GetLeftEdgeDate()
        {
            try
            {
                if (ChartControl != null)
                {
                    DateTime left = ChartControl.FirstTimePainted;
                    if (left != DateTime.MinValue)
                        return left.Date;
                }
            }
            catch { /* ignore */ }

            // Fallbacks
            if (CurrentBar >= 0) return Times[0][0].Date;
            return DateTime.Now.Date;
        }

        private ScrollBar FindHorizontalScrollBar(DependencyObject root)
        {
            if (root == null) return null;
            var q = new Queue<DependencyObject>();
            q.Enqueue(root);
            while (q.Count > 0)
            {
                var d = q.Dequeue();
                int n = VisualTreeHelper.GetChildrenCount(d);
                for (int i = 0; i < n; i++)
                {
                    var child = VisualTreeHelper.GetChild(d, i);
                    if (child is ScrollBar sb && sb.Orientation == Orientation.Horizontal)
                        return sb;
                    q.Enqueue(child);
                }
            }
            return null;
        }

        private bool TryParseTime(string s, out DateTime dt)
        {
            if (DateTime.TryParseExact(s, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;
            return DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out dt);
        }

        private void DPrint(string msg)
        {
            if (!DebugMode) return;
            try { Print($"[JumpToTimeButton] {msg}"); } catch { }
        }
    }
}
