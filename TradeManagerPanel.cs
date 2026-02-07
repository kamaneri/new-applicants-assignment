#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.Core.FloatingPoint;
#endregion

//This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
    /// <summary>
    /// Trade Manager Panel for NinjaTrader 8
    /// Provides visual stop loss & profit target projection, automated R:R targeting,
    /// position sizing, auto breakeven, and one-click trade execution.
    /// </summary>
    public class TradeManagerPanel : Indicator
    {
        #region Variables
        
        // UI Components
        private Grid mainGrid;
        private Button buyButton;
        private Button sellButton;
        private CheckBox autoBECheckBox;
        private ComboBox targetCountCombo;
        private ComboBox beModeCombo;
        private TextBox accountRiskTextBox;
        private TextBox tickBufferTextBox;
        private TextBox beTicksTextBox;
        private TextBox beBufferTextBox;
        private TextBox target1PctTextBox;
        private TextBox target2PctTextBox;
        private TextBox target3PctTextBox;
        
        // Visual elements for levels
        private System.Windows.Shapes.Line stopLossLine;
        private System.Windows.Shapes.Line target1Line;
        private System.Windows.Shapes.Line target2Line;
        private System.Windows.Shapes.Line target3Line;
        private System.Windows.Shapes.Line target4Line;
        
        // Chart Control
        private Chart chartWindow;
        private ChartControl chartControl;
        private ChartScale chartScale;
        
        // Trade Variables
        private double entryPrice;
        private double stopLossPrice;
        private double stopLossDistance;
        private double target1Price;
        private double target2Price;
        private double target3Price;
        private double target4Price;
        
        private int totalContracts;
        private bool isLongPosition;
        private bool isPanelActive;
        private Order entryOrder;
        private Order stopOrder;
        private List<Order> targetOrders;
        
        // Breakeven tracking
        private bool breakEvenTriggered;
        private double breakEvenTriggerPrice;
        
        #endregion
        
        #region Properties
        
        [NinjaScriptProperty]
        [Display(Name = "Account Risk %", Description = "Percentage of account to risk per trade", Order = 1, GroupName = "Risk Management")]
        [Range(0.1, 10.0)]
        public double AccountRiskPercent { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Stop Loss Tick Buffer", Description = "Additional ticks to add to stop loss beyond candlestick range", Order = 2, GroupName = "Risk Management")]
        [Range(1, 100)]
        public int StopLossTickBuffer { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Number of Targets", Description = "Number of profit targets (1-3)", Order = 3, GroupName = "Targets")]
        [Range(1, 3)]
        public int NumberOfTargets { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Target 1 %", Description = "Percentage of contracts for Target 1 (1R)", Order = 4, GroupName = "Targets")]
        [Range(1, 100)]
        public int Target1Percent { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Target 2 %", Description = "Percentage of contracts for Target 2 (2R)", Order = 5, GroupName = "Targets")]
        [Range(1, 100)]
        public int Target2Percent { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Target 3 %", Description = "Percentage of contracts for Target 3 (3R)", Order = 6, GroupName = "Targets")]
        [Range(1, 100)]
        public int Target3Percent { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Enable Auto Breakeven", Description = "Enable automatic breakeven functionality", Order = 7, GroupName = "Breakeven")]
        public bool EnableAutoBreakeven { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Breakeven Mode", Description = "Fixed Ticks or Risk Based (1R)", Order = 8, GroupName = "Breakeven")]
        public BreakevenMode BEMode { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "BE Fixed Ticks", Description = "Number of ticks in profit to trigger breakeven (Fixed Mode)", Order = 9, GroupName = "Breakeven")]
        [Range(1, 200)]
        public int BreakevenTicks { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "BE Tick Buffer", Description = "Additional ticks beyond breakeven to set stop", Order = 10, GroupName = "Breakeven")]
        [Range(0, 50)]
        public int BreakevenBuffer { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Show Visual Levels", Description = "Display stop loss and target lines on chart", Order = 11, GroupName = "Display")]
        public bool ShowVisualLevels { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Panel X Position", Description = "Horizontal position of panel", Order = 12, GroupName = "Display")]
        [Range(0, 2000)]
        public int PanelX { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Panel Y Position", Description = "Vertical position of panel", Order = 13, GroupName = "Display")]
        [Range(0, 2000)]
        public int PanelY { get; set; }
        
        #endregion
        
        #region Enums
        
        public enum BreakevenMode
        {
            [Description("Fixed Ticks")]
            FixedTicks,
            [Description("Risk Based (1R)")]
            RiskBased
        }
        
        #endregion
        
        #region NinjaScript Methods
        
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Trade Manager Panel - Visual stop loss, profit targets, R:R targeting, auto breakeven, and one-click execution";
                Name = "TradeManagerPanel";
                Calculate = Calculate.OnPriceChange;
                IsOverlay = true;
                DisplayInDataBox = false;
                PaintPriceMarkers = false;
                IsSuspendedWhileInactive = false;
                
                // Default values
                AccountRiskPercent = 1.0;
                StopLossTickBuffer = 2;
                NumberOfTargets = 3;
                Target1Percent = 50;
                Target2Percent = 30;
                Target3Percent = 20;
                EnableAutoBreakeven = true;
                BEMode = BreakevenMode.RiskBased;
                BreakevenTicks = 10;
                BreakevenBuffer = 2;
                ShowVisualLevels = true;
                PanelX = 10;
                PanelY = 100;
            }
            else if (State == State.Historical)
            {
                if (ChartControl != null)
                {
                    ChartControl.Dispatcher.InvokeAsync(() => CreateWPFControls());
                }
            }
            else if (State == State.Terminated)
            {
                if (ChartControl != null)
                {
                    ChartControl.Dispatcher.InvokeAsync(() => DisposeWPFControls());
                }
            }
        }
        
        protected override void OnBarUpdate()
        {
            if (CurrentBar < 1)
                return;
            
            // Update visual levels if panel is active and position exists
            if (isPanelActive)
            {
                UpdateVisualLevels();
            }
            
            // Check for auto breakeven trigger
            if (EnableAutoBreakeven && entryOrder != null && Position.MarketPosition != MarketPosition.Flat && !breakEvenTriggered)
            {
                CheckBreakevenTrigger();
            }
        }
        
        #endregion
        
        #region WPF UI Creation
        
        private void CreateWPFControls()
        {
            chartWindow = Window.GetWindow(ChartControl.Parent) as Chart;
            chartControl = ChartControl;
            chartScale = ChartPanel.ChartScale;
            
            if (chartWindow == null)
                return;
            
            // Create main grid
            mainGrid = new Grid
            {
                Width = 280,
                MinHeight = 450,
                Background = new SolidColorBrush(Color.FromArgb(220, 30, 30, 40)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(PanelX, PanelY, 0, 0)
            };
            
            // Define grid rows
            for (int i = 0; i < 20; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            int row = 0;
            
            // Title
            AddLabel(mainGrid, "TRADE MANAGER", row++, 18, FontWeights.Bold, Brushes.White);
            AddSeparator(mainGrid, row++);
            
            // Risk Management Section
            AddLabel(mainGrid, "Risk Management", row++, 14, FontWeights.SemiBold, Brushes.LightBlue);
            
            // Account Risk %
            Grid riskGrid = new Grid();
            riskGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            riskGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(riskGrid, "Account Risk %:", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            accountRiskTextBox = new TextBox
            {
                Text = AccountRiskPercent.ToString("F2"),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(accountRiskTextBox, 1);
            riskGrid.Children.Add(accountRiskTextBox);
            Grid.SetRow(riskGrid, row++);
            mainGrid.Children.Add(riskGrid);
            
            // Tick Buffer
            Grid bufferGrid = new Grid();
            bufferGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            bufferGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(bufferGrid, "SL Tick Buffer:", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            tickBufferTextBox = new TextBox
            {
                Text = StopLossTickBuffer.ToString(),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(tickBufferTextBox, 1);
            bufferGrid.Children.Add(tickBufferTextBox);
            Grid.SetRow(bufferGrid, row++);
            mainGrid.Children.Add(bufferGrid);
            
            AddSeparator(mainGrid, row++);
            
            // Targets Section
            AddLabel(mainGrid, "Profit Targets", row++, 14, FontWeights.SemiBold, Brushes.LightGreen);
            
            // Number of targets
            Grid targetCountGrid = new Grid();
            targetCountGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            targetCountGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(targetCountGrid, "Number of Targets:", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            targetCountCombo = new ComboBox
            {
                Height = 25,
                Margin = new Thickness(2)
            };
            targetCountCombo.Items.Add("1 Target");
            targetCountCombo.Items.Add("2 Targets");
            targetCountCombo.Items.Add("3 Targets");
            targetCountCombo.SelectedIndex = NumberOfTargets - 1;
            targetCountCombo.SelectionChanged += TargetCountCombo_SelectionChanged;
            Grid.SetColumn(targetCountCombo, 1);
            targetCountGrid.Children.Add(targetCountCombo);
            Grid.SetRow(targetCountGrid, row++);
            mainGrid.Children.Add(targetCountGrid);
            
            // Target percentages
            Grid target1Grid = new Grid();
            target1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            target1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(target1Grid, "Target 1 % (1R):", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            target1PctTextBox = new TextBox
            {
                Text = Target1Percent.ToString(),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(target1PctTextBox, 1);
            target1Grid.Children.Add(target1PctTextBox);
            Grid.SetRow(target1Grid, row++);
            mainGrid.Children.Add(target1Grid);
            
            Grid target2Grid = new Grid();
            target2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            target2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(target2Grid, "Target 2 % (2R):", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            target2PctTextBox = new TextBox
            {
                Text = Target2Percent.ToString(),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(target2PctTextBox, 1);
            target2Grid.Children.Add(target2PctTextBox);
            Grid.SetRow(target2Grid, row++);
            mainGrid.Children.Add(target2Grid);
            
            Grid target3Grid = new Grid();
            target3Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            target3Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(target3Grid, "Target 3 % (3R):", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            target3PctTextBox = new TextBox
            {
                Text = Target3Percent.ToString(),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(target3PctTextBox, 1);
            target3Grid.Children.Add(target3PctTextBox);
            Grid.SetRow(target3Grid, row++);
            mainGrid.Children.Add(target3Grid);
            
            AddSeparator(mainGrid, row++);
            
            // Breakeven Section
            AddLabel(mainGrid, "Auto Breakeven", row++, 14, FontWeights.SemiBold, Brushes.Yellow);
            
            // Enable Auto BE
            autoBECheckBox = new CheckBox
            {
                Content = "Enable Auto Breakeven",
                IsChecked = EnableAutoBreakeven,
                Foreground = Brushes.LightGray,
                Margin = new Thickness(5, 5, 5, 5)
            };
            Grid.SetRow(autoBECheckBox, row++);
            mainGrid.Children.Add(autoBECheckBox);
            
            // BE Mode
            Grid beModeGrid = new Grid();
            beModeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            beModeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(beModeGrid, "BE Mode:", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            beModeCombo = new ComboBox
            {
                Height = 25,
                Margin = new Thickness(2)
            };
            beModeCombo.Items.Add("Fixed Ticks");
            beModeCombo.Items.Add("Risk Based (1R)");
            beModeCombo.SelectedIndex = (int)BEMode;
            Grid.SetColumn(beModeCombo, 1);
            beModeGrid.Children.Add(beModeCombo);
            Grid.SetRow(beModeGrid, row++);
            mainGrid.Children.Add(beModeGrid);
            
            // BE Ticks
            Grid beTicksGrid = new Grid();
            beTicksGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            beTicksGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(beTicksGrid, "BE Ticks (Fixed):", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            beTicksTextBox = new TextBox
            {
                Text = BreakevenTicks.ToString(),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(beTicksTextBox, 1);
            beTicksGrid.Children.Add(beTicksTextBox);
            Grid.SetRow(beTicksGrid, row++);
            mainGrid.Children.Add(beTicksGrid);
            
            // BE Buffer
            Grid beBufferGrid = new Grid();
            beBufferGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            beBufferGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AddLabel(beBufferGrid, "BE Buffer:", 0, 11, FontWeights.Normal, Brushes.LightGray, 0);
            beBufferTextBox = new TextBox
            {
                Text = BreakevenBuffer.ToString(),
                Height = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(beBufferTextBox, 1);
            beBufferGrid.Children.Add(beBufferTextBox);
            Grid.SetRow(beBufferGrid, row++);
            mainGrid.Children.Add(beBufferGrid);
            
            AddSeparator(mainGrid, row++);
            
            // Execution Buttons
            AddLabel(mainGrid, "Execute Trade", row++, 14, FontWeights.SemiBold, Brushes.Orange);
            
            // Buy Button
            buyButton = new Button
            {
                Content = "BUY",
                Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(0, 150, 0)),
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 5, 5, 2),
                Cursor = Cursors.Hand
            };
            buyButton.Click += BuyButton_Click;
            Grid.SetRow(buyButton, row++);
            mainGrid.Children.Add(buyButton);
            
            // Sell Button
            sellButton = new Button
            {
                Content = "SELL",
                Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(200, 0, 0)),
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 2, 5, 5),
                Cursor = Cursors.Hand
            };
            sellButton.Click += SellButton_Click;
            Grid.SetRow(sellButton, row++);
            mainGrid.Children.Add(sellButton);
            
            // Add panel to chart
            if (UserControlCollection != null)
            {
                UserControlCollection.Add(mainGrid);
            }
            
            // Initialize target orders list
            targetOrders = new List<Order>();
        }
        
        private void AddLabel(Grid grid, string text, int row, double fontSize, FontWeight fontWeight, Brush foreground, int column = 0)
        {
            Label label = new Label
            {
                Content = text,
                FontSize = fontSize,
                FontWeight = fontWeight,
                Foreground = foreground,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 2, 5, 2)
            };
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
            grid.Children.Add(label);
        }
        
        private void AddSeparator(Grid grid, int row)
        {
            Border separator = new Border
            {
                Height = 1,
                Background = Brushes.Gray,
                Margin = new Thickness(5, 5, 5, 5)
            };
            Grid.SetRow(separator, row);
            grid.Children.Add(separator);
        }
        
        private void DisposeWPFControls()
        {
            if (mainGrid != null && UserControlCollection != null && UserControlCollection.Contains(mainGrid))
            {
                UserControlCollection.Remove(mainGrid);
            }
            
            RemoveVisualLevels();
        }
        
        #endregion
        
        #region Event Handlers
        
        private void TargetCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (targetCountCombo != null)
            {
                NumberOfTargets = targetCountCombo.SelectedIndex + 1;
            }
        }
        
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTrade(true);
        }
        
        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTrade(false);
        }
        
        #endregion
        
        #region Trade Execution
        
        private void ExecuteTrade(bool isLong)
        {
            try
            {
                // Get current values from UI
                UpdatePropertiesFromUI();
                
                // Validate inputs
                if (!ValidateInputs())
                {
                    Print("Invalid input values. Please check your settings.");
                    return;
                }
                
                // Check if already in position
                if (Position.MarketPosition != MarketPosition.Flat)
                {
                    Print("Already in a position. Close existing position before entering new trade.");
                    return;
                }
                
                isLongPosition = isLong;
                entryPrice = Close[0];
                
                // Calculate stop loss
                CalculateStopLoss();
                
                // Calculate targets
                CalculateTargets();
                
                // Calculate position size
                CalculatePositionSize();
                
                if (totalContracts <= 0)
                {
                    Print("Position size calculation resulted in 0 contracts. Check account balance and risk settings.");
                    return;
                }
                
                // Activate panel
                isPanelActive = true;
                breakEvenTriggered = false;
                
                // Place entry order
                if (isLong)
                {
                    entryOrder = EnterLong(totalContracts, "Entry");
                }
                else
                {
                    entryOrder = EnterShort(totalContracts, "Entry");
                }
                
                // Place stop loss order
                if (isLong)
                {
                    stopOrder = ExitLongStopMarket(0, true, totalContracts, stopLossPrice, "StopLoss", "Entry");
                }
                else
                {
                    stopOrder = ExitShortStopMarket(0, true, totalContracts, stopLossPrice, "StopLoss", "Entry");
                }
                
                // Place target orders
                PlaceTargetOrders();
                
                // Calculate breakeven trigger price
                if (EnableAutoBreakeven)
                {
                    if (BEMode == BreakevenMode.FixedTicks)
                    {
                        breakEvenTriggerPrice = isLong 
                            ? entryPrice + (BreakevenTicks * TickSize)
                            : entryPrice - (BreakevenTicks * TickSize);
                    }
                    else // Risk Based
                    {
                        breakEvenTriggerPrice = isLong
                            ? entryPrice + stopLossDistance
                            : entryPrice - stopLossDistance;
                    }
                }
                
                // Draw visual levels
                if (ShowVisualLevels)
                {
                    DrawVisualLevels();
                }
                
                Print(string.Format("Trade executed: {0} {1} contracts at {2}", 
                    isLong ? "LONG" : "SHORT", totalContracts, entryPrice));
                Print(string.Format("Stop Loss: {0} | Target 1: {1} | Target 2: {2} | Target 3: {3}", 
                    stopLossPrice, target1Price, target2Price, target3Price));
            }
            catch (Exception ex)
            {
                Print("Error executing trade: " + ex.Message);
            }
        }
        
        private void UpdatePropertiesFromUI()
        {
            if (accountRiskTextBox != null && double.TryParse(accountRiskTextBox.Text, out double riskPct))
                AccountRiskPercent = riskPct;
            
            if (tickBufferTextBox != null && int.TryParse(tickBufferTextBox.Text, out int buffer))
                StopLossTickBuffer = buffer;
            
            if (target1PctTextBox != null && int.TryParse(target1PctTextBox.Text, out int t1))
                Target1Percent = t1;
            
            if (target2PctTextBox != null && int.TryParse(target2PctTextBox.Text, out int t2))
                Target2Percent = t2;
            
            if (target3PctTextBox != null && int.TryParse(target3PctTextBox.Text, out int t3))
                Target3Percent = t3;
            
            if (autoBECheckBox != null)
                EnableAutoBreakeven = autoBECheckBox.IsChecked ?? false;
            
            if (beModeCombo != null)
                BEMode = (BreakevenMode)beModeCombo.SelectedIndex;
            
            if (beTicksTextBox != null && int.TryParse(beTicksTextBox.Text, out int beTicks))
                BreakevenTicks = beTicks;
            
            if (beBufferTextBox != null && int.TryParse(beBufferTextBox.Text, out int beBuffer))
                BreakevenBuffer = beBuffer;
        }
        
        private bool ValidateInputs()
        {
            if (AccountRiskPercent <= 0 || AccountRiskPercent > 10)
                return false;
            
            if (StopLossTickBuffer < 0)
                return false;
            
            // Validate target percentages sum to 100 for active targets
            int totalPct = 0;
            if (NumberOfTargets >= 1)
                totalPct += Target1Percent;
            if (NumberOfTargets >= 2)
                totalPct += Target2Percent;
            if (NumberOfTargets >= 3)
                totalPct += Target3Percent;
            
            if (totalPct != 100)
            {
                Print("Target percentages must sum to 100%. Current sum: " + totalPct);
                return false;
            }
            
            return true;
        }
        
        private void CalculateStopLoss()
        {
            // Get the most recent candlestick range
            double candleRange = High[0] - Low[0];
            
            // Add tick buffer
            double bufferAmount = StopLossTickBuffer * TickSize;
            stopLossDistance = candleRange + bufferAmount;
            
            // Calculate stop loss price
            if (isLongPosition)
            {
                stopLossPrice = entryPrice - stopLossDistance;
            }
            else
            {
                stopLossPrice = entryPrice + stopLossDistance;
            }
            
            // Round to tick size
            stopLossPrice = Instrument.MasterInstrument.RoundToTickSize(stopLossPrice);
        }
        
        private void CalculateTargets()
        {
            // Target 1 = 1R (same distance as stop loss)
            // Target 2 = 2R (2x stop loss distance)
            // Target 3 = 3R (3x stop loss distance)
            // Target 4 = 4R (4x stop loss distance) - for visual display
            
            if (isLongPosition)
            {
                target1Price = entryPrice + stopLossDistance;
                target2Price = entryPrice + (stopLossDistance * 2);
                target3Price = entryPrice + (stopLossDistance * 3);
                target4Price = entryPrice + (stopLossDistance * 4);
            }
            else
            {
                target1Price = entryPrice - stopLossDistance;
                target2Price = entryPrice - (stopLossDistance * 2);
                target3Price = entryPrice - (stopLossDistance * 3);
                target4Price = entryPrice - (stopLossDistance * 4);
            }
            
            // Round to tick size
            target1Price = Instrument.MasterInstrument.RoundToTickSize(target1Price);
            target2Price = Instrument.MasterInstrument.RoundToTickSize(target2Price);
            target3Price = Instrument.MasterInstrument.RoundToTickSize(target3Price);
            target4Price = Instrument.MasterInstrument.RoundToTickSize(target4Price);
        }
        
        private void CalculatePositionSize()
        {
            // Get account balance
            double accountBalance = Account.Get(AccountItem.CashValue, Currency.UsDollar);
            
            // Calculate risk amount in dollars
            double riskAmount = accountBalance * (AccountRiskPercent / 100.0);
            
            // Calculate risk per contract
            double stopLossTicks = Math.Abs(stopLossDistance / TickSize);
            double riskPerContract = stopLossTicks * TickSize * Instrument.MasterInstrument.PointValue;
            
            // Calculate total contracts
            totalContracts = (int)Math.Floor(riskAmount / riskPerContract);
            
            // Ensure at least 1 contract
            if (totalContracts < 1)
                totalContracts = 1;
        }
        
        private void PlaceTargetOrders()
        {
            targetOrders.Clear();
            
            // Calculate contract distribution
            int target1Contracts = (int)Math.Round(totalContracts * (Target1Percent / 100.0));
            int target2Contracts = (int)Math.Round(totalContracts * (Target2Percent / 100.0));
            int target3Contracts = totalContracts - target1Contracts - target2Contracts; // Remainder goes to target 3
            
            // Adjust if needed to ensure total equals totalContracts
            if (NumberOfTargets == 1)
            {
                target1Contracts = totalContracts;
                target2Contracts = 0;
                target3Contracts = 0;
            }
            else if (NumberOfTargets == 2)
            {
                target2Contracts = totalContracts - target1Contracts;
                target3Contracts = 0;
            }
            
            // Place target orders
            if (target1Contracts > 0 && NumberOfTargets >= 1)
            {
                Order target1Order;
                if (isLongPosition)
                    target1Order = ExitLongLimit(0, true, target1Contracts, target1Price, "Target1", "Entry");
                else
                    target1Order = ExitShortLimit(0, true, target1Contracts, target1Price, "Target1", "Entry");
                
                targetOrders.Add(target1Order);
            }
            
            if (target2Contracts > 0 && NumberOfTargets >= 2)
            {
                Order target2Order;
                if (isLongPosition)
                    target2Order = ExitLongLimit(0, true, target2Contracts, target2Price, "Target2", "Entry");
                else
                    target2Order = ExitShortLimit(0, true, target2Contracts, target2Price, "Target2", "Entry");
                
                targetOrders.Add(target2Order);
            }
            
            if (target3Contracts > 0 && NumberOfTargets >= 3)
            {
                Order target3Order;
                if (isLongPosition)
                    target3Order = ExitLongLimit(0, true, target3Contracts, target3Price, "Target3", "Entry");
                else
                    target3Order = ExitShortLimit(0, true, target3Contracts, target3Price, "Target3", "Entry");
                
                targetOrders.Add(target3Order);
            }
        }
        
        #endregion
        
        #region Breakeven Logic
        
        private void CheckBreakevenTrigger()
        {
            double currentPrice = Close[0];
            
            bool triggerBE = false;
            
            if (isLongPosition)
            {
                if (currentPrice >= breakEvenTriggerPrice)
                    triggerBE = true;
            }
            else
            {
                if (currentPrice <= breakEvenTriggerPrice)
                    triggerBE = true;
            }
            
            if (triggerBE)
            {
                MoveStopToBreakeven();
                breakEvenTriggered = true;
            }
        }
        
        private void MoveStopToBreakeven()
        {
            // Calculate new stop price with buffer
            double newStopPrice;
            if (isLongPosition)
            {
                newStopPrice = entryPrice + (BreakevenBuffer * TickSize);
            }
            else
            {
                newStopPrice = entryPrice - (BreakevenBuffer * TickSize);
            }
            
            newStopPrice = Instrument.MasterInstrument.RoundToTickSize(newStopPrice);
            
            // Move stop loss
            if (stopOrder != null && stopOrder.OrderState == OrderState.Working)
            {
                ChangeOrder(stopOrder, stopOrder.Quantity, 0, newStopPrice);
                Print(string.Format("Stop moved to breakeven + {0} ticks at {1}", BreakevenBuffer, newStopPrice));
            }
        }
        
        #endregion
        
        #region Visual Levels
        
        private void DrawVisualLevels()
        {
            if (ChartControl == null || chartScale == null)
                return;
            
            ChartControl.Dispatcher.InvokeAsync(() =>
            {
                RemoveVisualLevels();
                
                // Draw Stop Loss Line (Red)
                stopLossLine = CreateLine(stopLossPrice, Brushes.Red, 2);
                if (stopLossLine != null && UserControlCollection != null)
                    UserControlCollection.Add(stopLossLine);
                
                // Draw Target Lines (Green)
                if (NumberOfTargets >= 1)
                {
                    target1Line = CreateLine(target1Price, Brushes.LimeGreen, 2);
                    if (target1Line != null && UserControlCollection != null)
                        UserControlCollection.Add(target1Line);
                }
                
                if (NumberOfTargets >= 2)
                {
                    target2Line = CreateLine(target2Price, Brushes.LimeGreen, 1.5);
                    if (target2Line != null && UserControlCollection != null)
                        UserControlCollection.Add(target2Line);
                }
                
                if (NumberOfTargets >= 3)
                {
                    target3Line = CreateLine(target3Price, Brushes.LimeGreen, 1.5);
                    if (target3Line != null && UserControlCollection != null)
                        UserControlCollection.Add(target3Line);
                }
                
                // Draw 4R target for visual reference
                target4Line = CreateLine(target4Price, Brushes.Yellow, 1, true);
                if (target4Line != null && UserControlCollection != null)
                    UserControlCollection.Add(target4Line);
            });
        }
        
        private System.Windows.Shapes.Line CreateLine(double price, Brush color, double thickness, bool dashed = false)
        {
            if (ChartControl == null || chartScale == null)
                return null;
            
            try
            {
                int y = chartScale.GetYByValue(price);
                
                var line = new System.Windows.Shapes.Line
                {
                    X1 = 0,
                    X2 = ChartControl.ActualWidth,
                    Y1 = y,
                    Y2 = y,
                    Stroke = color,
                    StrokeThickness = thickness,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                
                if (dashed)
                {
                    line.StrokeDashArray = new DoubleCollection { 4, 2 };
                }
                
                return line;
            }
            catch
            {
                return null;
            }
        }
        
        private void UpdateVisualLevels()
        {
            if (!ShowVisualLevels || ChartControl == null)
                return;
            
            ChartControl.Dispatcher.InvokeAsync(() =>
            {
                // Update line positions based on current chart scale
                UpdateLine(stopLossLine, stopLossPrice);
                UpdateLine(target1Line, target1Price);
                UpdateLine(target2Line, target2Price);
                UpdateLine(target3Line, target3Price);
                UpdateLine(target4Line, target4Price);
            });
        }
        
        private void UpdateLine(System.Windows.Shapes.Line line, double price)
        {
            if (line != null && chartScale != null && ChartControl != null)
            {
                try
                {
                    int y = chartScale.GetYByValue(price);
                    line.Y1 = y;
                    line.Y2 = y;
                    line.X2 = ChartControl.ActualWidth;
                }
                catch
                {
                    // Ignore errors during update
                }
            }
        }
        
        private void RemoveVisualLevels()
        {
            if (UserControlCollection == null)
                return;
            
            if (stopLossLine != null && UserControlCollection.Contains(stopLossLine))
                UserControlCollection.Remove(stopLossLine);
            
            if (target1Line != null && UserControlCollection.Contains(target1Line))
                UserControlCollection.Remove(target1Line);
            
            if (target2Line != null && UserControlCollection.Contains(target2Line))
                UserControlCollection.Remove(target2Line);
            
            if (target3Line != null && UserControlCollection.Contains(target3Line))
                UserControlCollection.Remove(target3Line);
            
            if (target4Line != null && UserControlCollection.Contains(target4Line))
                UserControlCollection.Remove(target4Line);
            
            stopLossLine = null;
            target1Line = null;
            target2Line = null;
            target3Line = null;
            target4Line = null;
        }
        
        #endregion
    }
}
