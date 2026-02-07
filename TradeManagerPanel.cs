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

//This namespace holds Add ons in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.AddOns
{
    /// <summary>
    /// Trade Manager Panel for NinjaTrader 8
    /// Provides automated risk management with visual stop loss and profit targets
    /// </summary>
    public class TradeManagerPanel : NinjaTrader.NinjaScript.AddOnBase
    {
        #region Variables
        
        // Chart and Account references
        private Chart chartWindow;
        private Account account;
        private Instrument instrument;
        
        // UI Components
        private Grid mainGrid;
        private Border mainBorder;
        private Button buyButton;
        private Button sellButton;
        private CheckBox autoBreakevenCheckBox;
        private ComboBox breakevenModeComboBox;
        private ComboBox targetCountComboBox;
        private TextBox riskPercentageTextBox;
        private TextBox stopLossBufferTextBox;
        private TextBox breakevenTicksTextBox;
        private TextBox breakevenBufferTextBox;
        private TextBox target1PercentTextBox;
        private TextBox target2PercentTextBox;
        private TextBox target3PercentTextBox;
        private Label statusLabel;
        private Label infoLabel;
        
        // Trading Variables
        private double entryPrice;
        private double stopLossPrice;
        private double[] profitTargets = new double[3];
        private int totalContracts;
        private int[] contractsPerTarget = new int[3];
        private bool isLongPosition;
        private bool autoBreakevenEnabled;
        private int breakevenMode; // 1 = Fixed Ticks, 2 = 1R + Buffer
        private int targetCount = 3;
        
        // Configuration
        private double riskPercentage = 1.0; // Default 1% risk
        private int stopLossBufferTicks = 2; // Buffer ticks added to stop loss
        private int breakevenTicks = 10; // Ticks for fixed breakeven mode
        private int breakevenBufferTicks = 5; // Buffer ticks for breakeven
        private double[] targetPercentages = new double[] { 0.5, 0.3, 0.2 }; // Default 50%, 30%, 20%
        
        // Visual Elements
        private Dictionary<string, IDrawingTool> visualElements = new Dictionary<string, IDrawingTool>();
        
        // Order tracking
        private Order stopLossOrder;
        private List<Order> targetOrders = new List<Order>();
        private Position currentPosition;
        
        #endregion
        
        #region OnStateChange
        
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Advanced Trade Manager Panel with automated risk management";
                Name = "TradeManagerPanel";
            }
            else if (State == State.Configure)
            {
            }
            else if (State == State.Active)
            {
                // Initialize when active
            }
            else if (State == State.Terminated)
            {
                // Cleanup
                if (chartWindow != null && mainGrid != null)
                {
                    chartWindow.Dispatcher.InvokeAsync(() =>
                    {
                        RemovePanelFromChart();
                    });
                }
            }
        }
        
        #endregion
        
        #region Panel Creation and UI Setup
        
        /// <summary>
        /// Creates and adds the trade manager panel to the chart
        /// </summary>
        public void AddPanelToChart(Chart chart)
        {
            if (chart == null)
                return;
                
            chartWindow = chart;
            
            chartWindow.Dispatcher.InvokeAsync(() =>
            {
                CreateMainPanel();
                
                // Add to chart's main grid
                if (chartWindow.MainTabControl != null)
                {
                    Grid chartGrid = chartWindow.MainTabControl.Parent as Grid;
                    if (chartGrid != null)
                    {
                        chartGrid.Children.Add(mainBorder);
                    }
                }
            });
        }
        
        /// <summary>
        /// Creates the main UI panel with all controls
        /// </summary>
        private void CreateMainPanel()
        {
            // Main border container
            mainBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(230, 30, 30, 30)),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 320
            };
            
            // Main grid layout
            mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            int row = 0;
            
            // Title
            AddTitleLabel(mainGrid, "Trade Manager Panel", row++);
            
            // Risk Percentage Section
            AddSectionLabel(mainGrid, "Risk Management", row++);
            AddLabeledTextBox(mainGrid, "Risk % of Account:", ref riskPercentageTextBox, riskPercentage.ToString(), row++);
            AddLabeledTextBox(mainGrid, "Stop Buffer (ticks):", ref stopLossBufferTextBox, stopLossBufferTicks.ToString(), row++);
            
            // Profit Targets Section
            AddSectionLabel(mainGrid, "Profit Targets", row++);
            AddTargetCountSelector(mainGrid, row++);
            AddLabeledTextBox(mainGrid, "Target 1 %:", ref target1PercentTextBox, "50", row++);
            AddLabeledTextBox(mainGrid, "Target 2 %:", ref target2PercentTextBox, "30", row++);
            AddLabeledTextBox(mainGrid, "Target 3 %:", ref target3PercentTextBox, "20", row++);
            
            // Auto Breakeven Section
            AddSectionLabel(mainGrid, "Auto Breakeven", row++);
            AddBreakevenControls(mainGrid, row++);
            AddLabeledTextBox(mainGrid, "BE Ticks (Mode 1):", ref breakevenTicksTextBox, breakevenTicks.ToString(), row++);
            AddLabeledTextBox(mainGrid, "BE Buffer (ticks):", ref breakevenBufferTextBox, breakevenBufferTicks.ToString(), row++);
            
            // Execution Buttons
            AddExecutionButtons(mainGrid, row++);
            
            // Status Label
            statusLabel = new Label
            {
                Content = "Ready",
                Foreground = new SolidColorBrush(Colors.LightGreen),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 0, 0)
            };
            Grid.SetRow(statusLabel, row++);
            mainGrid.Children.Add(statusLabel);
            
            // Info Label
            infoLabel = new Label
            {
                Content = "Panel Active",
                Foreground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 10,
                Margin = new Thickness(0, 2, 0, 0)
            };
            Grid.SetRow(infoLabel, row++);
            mainGrid.Children.Add(infoLabel);
            
            mainBorder.Child = mainGrid;
        }
        
        /// <summary>
        /// Adds a title label to the grid
        /// </summary>
        private void AddTitleLabel(Grid grid, string text, int row)
        {
            Label label = new Label
            {
                Content = text,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(label, row);
            grid.Children.Add(label);
        }
        
        /// <summary>
        /// Adds a section label to the grid
        /// </summary>
        private void AddSectionLabel(Grid grid, string text, int row)
        {
            Label label = new Label
            {
                Content = text,
                Foreground = new SolidColorBrush(Colors.LightBlue),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 0, 2)
            };
            Grid.SetRow(label, row);
            grid.Children.Add(label);
        }
        
        /// <summary>
        /// Adds a labeled textbox to the grid
        /// </summary>
        private void AddLabeledTextBox(Grid grid, string labelText, ref TextBox textBox, string defaultValue, int row)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 2)
            };
            
            Label label = new Label
            {
                Content = labelText,
                Foreground = new SolidColorBrush(Colors.LightGray),
                Width = 150,
                FontSize = 11
            };
            
            textBox = new TextBox
            {
                Text = defaultValue,
                Width = 140,
                Height = 22,
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                Padding = new Thickness(3)
            };
            
            panel.Children.Add(label);
            panel.Children.Add(textBox);
            Grid.SetRow(panel, row);
            grid.Children.Add(panel);
        }
        
        /// <summary>
        /// Adds target count selector dropdown
        /// </summary>
        private void AddTargetCountSelector(Grid grid, int row)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 2)
            };
            
            Label label = new Label
            {
                Content = "Number of Targets:",
                Foreground = new SolidColorBrush(Colors.LightGray),
                Width = 150,
                FontSize = 11
            };
            
            targetCountComboBox = new ComboBox
            {
                Width = 140,
                Height = 22,
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.Gray)
            };
            
            targetCountComboBox.Items.Add(new ComboBoxItem { Content = "1 Target", Tag = 1 });
            targetCountComboBox.Items.Add(new ComboBoxItem { Content = "2 Targets", Tag = 2 });
            targetCountComboBox.Items.Add(new ComboBoxItem { Content = "3 Targets", Tag = 3 });
            targetCountComboBox.SelectedIndex = 2; // Default to 3 targets
            
            targetCountComboBox.SelectionChanged += TargetCountComboBox_SelectionChanged;
            
            panel.Children.Add(label);
            panel.Children.Add(targetCountComboBox);
            Grid.SetRow(panel, row);
            grid.Children.Add(panel);
        }
        
        /// <summary>
        /// Adds breakeven control options
        /// </summary>
        private void AddBreakevenControls(Grid grid, int row)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 2)
            };
            
            autoBreakevenCheckBox = new CheckBox
            {
                Content = "Enable Auto BE",
                Foreground = new SolidColorBrush(Colors.LightGray),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                IsChecked = false
            };
            
            breakevenModeComboBox = new ComboBox
            {
                Width = 150,
                Height = 22,
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.Gray)
            };
            
            breakevenModeComboBox.Items.Add(new ComboBoxItem { Content = "Fixed Ticks", Tag = 1 });
            breakevenModeComboBox.Items.Add(new ComboBoxItem { Content = "1R + Buffer", Tag = 2 });
            breakevenModeComboBox.SelectedIndex = 0;
            
            panel.Children.Add(autoBreakevenCheckBox);
            panel.Children.Add(breakevenModeComboBox);
            Grid.SetRow(panel, row);
            grid.Children.Add(panel);
        }
        
        /// <summary>
        /// Adds Buy and Sell execution buttons
        /// </summary>
        private void AddExecutionButtons(Grid grid, int row)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 5)
            };
            
            buyButton = new Button
            {
                Content = "BUY",
                Width = 130,
                Height = 35,
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0)),
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 10, 0),
                BorderBrush = new SolidColorBrush(Colors.DarkGreen),
                BorderThickness = new Thickness(2)
            };
            buyButton.Click += BuyButton_Click;
            
            sellButton = new Button
            {
                Content = "SELL",
                Width = 130,
                Height = 35,
                Background = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0)),
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                BorderBrush = new SolidColorBrush(Colors.DarkRed),
                BorderThickness = new Thickness(2)
            };
            sellButton.Click += SellButton_Click;
            
            panel.Children.Add(buyButton);
            panel.Children.Add(sellButton);
            Grid.SetRow(panel, row);
            grid.Children.Add(panel);
        }
        
        /// <summary>
        /// Removes the panel from the chart
        /// </summary>
        private void RemovePanelFromChart()
        {
            if (chartWindow != null && mainBorder != null)
            {
                Grid chartGrid = chartWindow.MainTabControl.Parent as Grid;
                if (chartGrid != null && chartGrid.Children.Contains(mainBorder))
                {
                    chartGrid.Children.Remove(mainBorder);
                }
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handler for target count selection change
        /// </summary>
        private void TargetCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (targetCountComboBox.SelectedItem != null)
            {
                ComboBoxItem item = targetCountComboBox.SelectedItem as ComboBoxItem;
                targetCount = (int)item.Tag;
                UpdateTargetFieldsVisibility();
            }
        }
        
        /// <summary>
        /// Updates visibility of target percentage fields based on target count
        /// </summary>
        private void UpdateTargetFieldsVisibility()
        {
            if (target2PercentTextBox != null)
                target2PercentTextBox.IsEnabled = targetCount >= 2;
            if (target3PercentTextBox != null)
                target3PercentTextBox.IsEnabled = targetCount >= 3;
        }
        
        /// <summary>
        /// Handler for Buy button click
        /// </summary>
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTrade(true); // true = Long position
        }
        
        /// <summary>
        /// Handler for Sell button click
        /// </summary>
        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTrade(false); // false = Short position
        }
        
        #endregion
        
        #region Trade Execution Logic
        
        /// <summary>
        /// Main trade execution method - calculates all values and places orders
        /// </summary>
        private void ExecuteTrade(bool isLong)
        {
            try
            {
                UpdateStatus("Calculating trade parameters...", Colors.Yellow);
                
                // Read user inputs
                if (!ReadUserInputs())
                {
                    UpdateStatus("Error: Invalid input values", Colors.Red);
                    return;
                }
                
                isLongPosition = isLong;
                
                // Get current market data
                if (!GetMarketData())
                {
                    UpdateStatus("Error: Unable to get market data", Colors.Red);
                    return;
                }
                
                // Calculate stop loss based on last candle + buffer
                CalculateStopLoss();
                
                // Calculate profit targets (1R, 2R, 3R)
                CalculateProfitTargets();
                
                // Calculate position size based on risk percentage
                CalculatePositionSize();
                
                // Display visual projections on chart
                DisplayVisualProjections();
                
                // Place the orders
                PlaceOrders();
                
                UpdateStatus($"Trade executed: {totalContracts} contracts", Colors.LightGreen);
                UpdateInfoLabel();
                
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}", Colors.Red);
            }
        }
        
        /// <summary>
        /// Reads and validates user inputs from UI fields
        /// </summary>
        private bool ReadUserInputs()
        {
            try
            {
                // Risk percentage
                if (!double.TryParse(riskPercentageTextBox.Text, out riskPercentage) || riskPercentage <= 0)
                    return false;
                
                // Stop loss buffer
                if (!int.TryParse(stopLossBufferTextBox.Text, out stopLossBufferTicks) || stopLossBufferTicks < 0)
                    return false;
                
                // Breakeven settings
                autoBreakevenEnabled = autoBreakevenCheckBox.IsChecked ?? false;
                if (breakevenModeComboBox.SelectedItem != null)
                {
                    ComboBoxItem item = breakevenModeComboBox.SelectedItem as ComboBoxItem;
                    breakevenMode = (int)item.Tag;
                }
                
                if (!int.TryParse(breakevenTicksTextBox.Text, out breakevenTicks) || breakevenTicks < 0)
                    return false;
                
                if (!int.TryParse(breakevenBufferTextBox.Text, out breakevenBufferTicks) || breakevenBufferTicks < 0)
                    return false;
                
                // Target percentages
                double target1, target2, target3;
                if (!double.TryParse(target1PercentTextBox.Text, out target1) || target1 <= 0 || target1 > 100)
                    return false;
                
                if (targetCount >= 2)
                {
                    if (!double.TryParse(target2PercentTextBox.Text, out target2) || target2 <= 0 || target2 > 100)
                        return false;
                }
                else
                    target2 = 0;
                
                if (targetCount >= 3)
                {
                    if (!double.TryParse(target3PercentTextBox.Text, out target3) || target3 <= 0 || target3 > 100)
                        return false;
                }
                else
                    target3 = 0;
                
                // Normalize percentages to sum to 1.0
                double totalPercentage = target1 + target2 + target3;
                targetPercentages[0] = target1 / totalPercentage;
                targetPercentages[1] = target2 / totalPercentage;
                targetPercentages[2] = target3 / totalPercentage;
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets current market data from chart
        /// </summary>
        private bool GetMarketData()
        {
            if (chartWindow == null || chartWindow.ActiveChartControl == null)
                return false;
            
            // Get current price
            entryPrice = chartWindow.ActiveChartControl.GetCurrentAsk();
            
            // Get instrument
            if (chartWindow.ActiveChartControl.BarsArray != null && chartWindow.ActiveChartControl.BarsArray.Length > 0)
            {
                instrument = chartWindow.ActiveChartControl.BarsArray[0].Instrument;
            }
            
            // Get account
            if (account == null && Account.All.Count > 0)
            {
                account = Account.All[0];
            }
            
            return entryPrice > 0 && instrument != null;
        }
        
        /// <summary>
        /// Calculates stop loss based on last candlestick size + buffer
        /// </summary>
        private void CalculateStopLoss()
        {
            if (chartWindow == null || chartWindow.ActiveChartControl == null || chartWindow.ActiveChartControl.BarsArray == null)
                return;
            
            var bars = chartWindow.ActiveChartControl.BarsArray[0];
            if (bars == null || bars.Count < 1)
                return;
            
            // Get the most recent completed candle
            int lastBarIndex = bars.Count - 2; // -2 to get last completed bar
            if (lastBarIndex < 0)
                lastBarIndex = 0;
            
            double lastCandleHigh = bars.GetHigh(lastBarIndex);
            double lastCandleLow = bars.GetLow(lastBarIndex);
            double candleSize = lastCandleHigh - lastCandleLow;
            
            // Add buffer in ticks
            double bufferAmount = stopLossBufferTicks * instrument.MasterInstrument.TickSize;
            double stopDistance = candleSize + bufferAmount;
            
            // Calculate stop loss price
            if (isLongPosition)
            {
                // For long: stop is below entry
                stopLossPrice = entryPrice - stopDistance;
            }
            else
            {
                // For short: stop is above entry
                stopLossPrice = entryPrice + stopDistance;
            }
        }
        
        /// <summary>
        /// Calculates profit targets at 1R, 2R, and 3R
        /// </summary>
        private void CalculateProfitTargets()
        {
            // R = Risk = distance from entry to stop
            double riskDistance = Math.Abs(entryPrice - stopLossPrice);
            
            if (isLongPosition)
            {
                // For long: targets are above entry
                profitTargets[0] = entryPrice + (riskDistance * 1.0); // 1R
                profitTargets[1] = entryPrice + (riskDistance * 2.0); // 2R
                profitTargets[2] = entryPrice + (riskDistance * 3.0); // 3R
            }
            else
            {
                // For short: targets are below entry
                profitTargets[0] = entryPrice - (riskDistance * 1.0); // 1R
                profitTargets[1] = entryPrice - (riskDistance * 2.0); // 2R
                profitTargets[2] = entryPrice - (riskDistance * 3.0); // 3R
            }
        }
        
        /// <summary>
        /// Calculates position size based on account risk percentage
        /// </summary>
        private void CalculatePositionSize()
        {
            if (account == null)
            {
                totalContracts = 1; // Default fallback
                contractsPerTarget[0] = 1;
                contractsPerTarget[1] = 0;
                contractsPerTarget[2] = 0;
                return;
            }
            
            // Get account balance
            double accountBalance = account.Get(AccountItem.CashValue, Currency.UsDollar);
            
            // Calculate dollar risk amount
            double dollarRisk = accountBalance * (riskPercentage / 100.0);
            
            // Calculate risk per contract (in dollars)
            double riskPerContract = Math.Abs(entryPrice - stopLossPrice) * instrument.MasterInstrument.PointValue;
            
            if (riskPerContract > 0)
            {
                // Calculate total contracts
                totalContracts = (int)Math.Floor(dollarRisk / riskPerContract);
                
                // Ensure at least 1 contract
                if (totalContracts < 1)
                    totalContracts = 1;
            }
            else
            {
                totalContracts = 1;
            }
            
            // Distribute contracts across targets based on percentages
            DistributeContracts();
        }
        
        /// <summary>
        /// Distributes contracts across profit targets based on percentages
        /// </summary>
        private void DistributeContracts()
        {
            int remainingContracts = totalContracts;
            
            // Allocate contracts to each target based on target count
            for (int i = 0; i < targetCount; i++)
            {
                if (i == targetCount - 1)
                {
                    // Last target gets all remaining contracts
                    contractsPerTarget[i] = remainingContracts;
                }
                else
                {
                    // Calculate contracts for this target
                    contractsPerTarget[i] = (int)Math.Floor(totalContracts * targetPercentages[i]);
                    remainingContracts -= contractsPerTarget[i];
                }
            }
            
            // Zero out unused targets
            for (int i = targetCount; i < 3; i++)
            {
                contractsPerTarget[i] = 0;
            }
            
            // Ensure at least 1 contract for first target if total > 0
            if (totalContracts > 0 && contractsPerTarget[0] == 0)
                contractsPerTarget[0] = 1;
        }
        
        /// <summary>
        /// Places all orders (entry, stop loss, and profit targets)
        /// </summary>
        private void PlaceOrders()
        {
            if (account == null || instrument == null)
                return;
            
            try
            {
                // Place market order for entry
                OrderAction action = isLongPosition ? OrderAction.Buy : OrderAction.SellShort;
                
                // Submit market order
                Order entryOrder = account.CreateOrder(
                    instrument,
                    action,
                    OrderType.Market,
                    OrderEntry.Manual,
                    TimeInForce.Day,
                    totalContracts,
                    0,
                    0,
                    "",
                    "TradeManager_Entry",
                    null,
                    null
                );
                
                account.Submit(new[] { entryOrder });
                
                // Place stop loss order
                PlaceStopLossOrder();
                
                // Place profit target orders
                PlaceProfitTargetOrders();
                
                // Set up breakeven monitoring if enabled
                if (autoBreakevenEnabled)
                {
                    MonitorForBreakeven();
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Order Error: {ex.Message}", Colors.Red);
            }
        }
        
        /// <summary>
        /// Places stop loss order
        /// </summary>
        private void PlaceStopLossOrder()
        {
            if (account == null || instrument == null)
                return;
            
            OrderAction stopAction = isLongPosition ? OrderAction.Sell : OrderAction.BuyToCover;
            
            stopLossOrder = account.CreateOrder(
                instrument,
                stopAction,
                OrderType.StopMarket,
                OrderEntry.Manual,
                TimeInForce.Day,
                totalContracts,
                0,
                stopLossPrice,
                "",
                "TradeManager_StopLoss",
                null,
                null
            );
            
            account.Submit(new[] { stopLossOrder });
        }
        
        /// <summary>
        /// Places profit target orders
        /// </summary>
        private void PlaceProfitTargetOrders()
        {
            if (account == null || instrument == null)
                return;
            
            OrderAction targetAction = isLongPosition ? OrderAction.Sell : OrderAction.BuyToCover;
            targetOrders.Clear();
            
            for (int i = 0; i < targetCount; i++)
            {
                if (contractsPerTarget[i] > 0)
                {
                    Order targetOrder = account.CreateOrder(
                        instrument,
                        targetAction,
                        OrderType.Limit,
                        OrderEntry.Manual,
                        TimeInForce.Day,
                        contractsPerTarget[i],
                        profitTargets[i],
                        0,
                        "",
                        $"TradeManager_Target{i + 1}",
                        null,
                        null
                    );
                    
                    account.Submit(new[] { targetOrder });
                    targetOrders.Add(targetOrder);
                }
            }
        }
        
        #endregion
        
        #region Visual Display
        
        /// <summary>
        /// Displays visual projections of stop loss and targets on chart
        /// </summary>
        private void DisplayVisualProjections()
        {
            if (chartWindow == null || chartWindow.ActiveChartControl == null)
                return;
            
            // Clear previous visual elements
            ClearVisualProjections();
            
            try
            {
                var chartControl = chartWindow.ActiveChartControl;
                int currentBar = chartControl.BarsArray[0].Count - 1;
                
                // Draw stop loss line
                DrawHorizontalLine("StopLoss", stopLossPrice, Brushes.Red, 2, currentBar);
                
                // Draw profit target lines
                Color[] targetColors = new Color[] 
                { 
                    Colors.LimeGreen,
                    Colors.Yellow,
                    Colors.Orange
                };
                
                for (int i = 0; i < targetCount; i++)
                {
                    DrawHorizontalLine(
                        $"Target{i + 1}",
                        profitTargets[i],
                        new SolidColorBrush(targetColors[i]),
                        2,
                        currentBar
                    );
                }
                
                // Draw entry line
                DrawHorizontalLine("Entry", entryPrice, Brushes.White, 1, currentBar);
                
            }
            catch (Exception ex)
            {
                // Visual display is non-critical
                System.Diagnostics.Debug.WriteLine($"Visual display error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Draws a horizontal line on the chart
        /// </summary>
        private void DrawHorizontalLine(string tag, double price, Brush color, int width, int barIndex)
        {
            if (chartWindow == null)
                return;
            
            try
            {
                // Create line using NinjaTrader drawing tools
                var line = Draw.HorizontalLine(
                    chartWindow.ActiveChartControl.OwnerChart,
                    tag,
                    price,
                    color
                );
                
                if (line != null)
                {
                    visualElements[tag] = line;
                }
            }
            catch
            {
                // Drawing may fail in some contexts, this is non-critical
            }
        }
        
        /// <summary>
        /// Clears all visual projections from the chart
        /// </summary>
        private void ClearVisualProjections()
        {
            foreach (var element in visualElements)
            {
                try
                {
                    // Remove drawing object
                    if (chartWindow != null && chartWindow.ActiveChartControl != null)
                    {
                        chartWindow.ActiveChartControl.OwnerChart.ChartObjects.Remove(element.Key);
                    }
                }
                catch
                {
                    // Removal may fail if object already removed
                }
            }
            
            visualElements.Clear();
        }
        
        #endregion
        
        #region Auto Breakeven Logic
        
        /// <summary>
        /// Monitors position for breakeven trigger conditions
        /// </summary>
        private void MonitorForBreakeven()
        {
            // This would typically be called on market data updates
            // For a complete implementation, subscribe to OnMarketData or OnBarUpdate
            
            if (chartWindow == null || chartWindow.ActiveChartControl == null)
                return;
            
            double currentPrice = isLongPosition ? 
                chartWindow.ActiveChartControl.GetCurrentBid() : 
                chartWindow.ActiveChartControl.GetCurrentAsk();
            
            bool shouldTriggerBreakeven = false;
            
            if (breakevenMode == 1)
            {
                // Mode 1: Fixed tick trigger
                double breakevenDistance = breakevenTicks * instrument.MasterInstrument.TickSize;
                
                if (isLongPosition)
                {
                    shouldTriggerBreakeven = currentPrice >= (entryPrice + breakevenDistance);
                }
                else
                {
                    shouldTriggerBreakeven = currentPrice <= (entryPrice - breakevenDistance);
                }
            }
            else if (breakevenMode == 2)
            {
                // Mode 2: 1R (risk-based) trigger
                double riskDistance = Math.Abs(entryPrice - stopLossPrice);
                
                if (isLongPosition)
                {
                    shouldTriggerBreakeven = currentPrice >= (entryPrice + riskDistance);
                }
                else
                {
                    shouldTriggerBreakeven = currentPrice <= (entryPrice - riskDistance);
                }
            }
            
            if (shouldTriggerBreakeven)
            {
                MoveStopToBreakeven();
            }
        }
        
        /// <summary>
        /// Moves stop loss to breakeven (entry price + buffer)
        /// </summary>
        private void MoveStopToBreakeven()
        {
            if (stopLossOrder == null || account == null)
                return;
            
            try
            {
                // Calculate breakeven price with buffer
                double bufferAmount = breakevenBufferTicks * instrument.MasterInstrument.TickSize;
                double breakevenPrice;
                
                if (isLongPosition)
                {
                    breakevenPrice = entryPrice + bufferAmount;
                }
                else
                {
                    breakevenPrice = entryPrice - bufferAmount;
                }
                
                // Modify stop loss order to new price
                stopLossOrder.StopPrice = breakevenPrice;
                account.Change(new[] { stopLossOrder });
                
                UpdateStatus($"Stop moved to breakeven + {breakevenBufferTicks} ticks", Colors.LightBlue);
                
                // Update visual
                DrawHorizontalLine("StopLoss", breakevenPrice, Brushes.Blue, 2, 
                    chartWindow.ActiveChartControl.BarsArray[0].Count - 1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Breakeven error: {ex.Message}");
            }
        }
        
        #endregion
        
        #region UI Update Helpers
        
        /// <summary>
        /// Updates the status label
        /// </summary>
        private void UpdateStatus(string message, Color color)
        {
            if (statusLabel != null)
            {
                chartWindow.Dispatcher.InvokeAsync(() =>
                {
                    statusLabel.Content = message;
                    statusLabel.Foreground = new SolidColorBrush(color);
                });
            }
        }
        
        /// <summary>
        /// Updates the info label with trade details
        /// </summary>
        private void UpdateInfoLabel()
        {
            if (infoLabel != null)
            {
                string info = $"Contracts: {totalContracts} | Targets: {targetCount} | R:R 1:{targetCount}";
                
                chartWindow.Dispatcher.InvokeAsync(() =>
                {
                    infoLabel.Content = info;
                });
            }
        }
        
        #endregion
    }
}
