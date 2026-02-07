# Trade Manager Panel for NinjaTrader 8

## Overview

This is a comprehensive Trade Manager Panel for NinjaTrader 8 that provides automated risk management with visual stop loss and profit target projections. The panel helps traders execute trades with precise risk-to-reward calculations, automated position sizing, and intelligent breakeven management.

## Features

### 1. Visual Stop Loss & Profit Target Projection
- Displays stop loss level visually on the chart
- Shows up to 3 profit targets (1R, 2R, 3R)
- Updates dynamically based on current chart price
- Color-coded lines for easy identification:
  - Red: Stop Loss
  - Lime Green: Target 1 (1R)
  - Yellow: Target 2 (2R)
  - Orange: Target 3 (3R)
  - White: Entry Price

### 2. Automatic Stop Loss Calculation
- Calculates stop loss based on the most recent candlestick size (High - Low)
- Adds a user-defined tick buffer for additional safety
- Automatically adjusts for long or short positions

### 3. Automated Risk-to-Reward (R:R) Targeting
- Calculates profit targets at 1R, 2R, and 3R multiples
- R = Risk distance (distance from entry to stop loss)
- Each target level is calculated automatically
- Contract allocation is customizable per target

### 4. Position Sizing Based on Account Percentage
- Risk a fixed percentage of account balance (e.g., 1%)
- Automatically calculates the number of contracts based on:
  - Stop loss distance
  - Risk percentage
  - Account balance
  - Instrument point value
- Distributes contracts across selected profit targets

### 5. Profit Target Toggle Options
- Choose between 1, 2, or 3 profit targets
- Contract distribution automatically adjusts:
  - 1 Target: 100% of contracts
  - 2 Targets: Customizable split (e.g., 60% / 40%)
  - 3 Targets: Customizable split (e.g., 50% / 30% / 20%)
- Percentage fields dynamically enable/disable based on selection

### 6. Auto Breakeven Function
Two breakeven modes available:

**Mode 1 - Fixed Tick Trigger:**
- Breakeven activates when price moves a user-defined number of ticks in favor
- Example: Set to 10 ticks, stop moves to breakeven when price moves 10 ticks in your favor

**Mode 2 - Risk-Based Trigger (1R):**
- Breakeven activates when price moves 1R in favor (equal to stop loss distance)
- Additional configurable tick buffer applied after trigger
- Example: If stop is 50 ticks away and price moves 50 ticks in favor, with +5 tick buffer, stop moves to entry + 5 ticks

### 7. Execution Buttons
- **BUY Button**: Executes long trades
- **SELL Button**: Executes short trades
- Both buttons automatically:
  - Calculate stop loss and targets
  - Size position based on risk
  - Place all orders
  - Apply breakeven logic
  - Display visual projections

## Installation Instructions

### Step 1: Prepare the File
1. Download the `TradeManagerPanel.cs` file
2. Ensure the file is saved on your computer

### Step 2: Import into NinjaTrader 8
1. Open NinjaTrader 8
2. Go to **Control Center**
3. Click **Tools** → **Import** → **NinjaScript Add-On**
4. Browse to the `TradeManagerPanel.cs` file
5. Click **Open** to import

### Step 3: Compile the Script
1. In Control Center, go to **Tools** → **Edit NinjaScript** → **Add-On**
2. Find `TradeManagerPanel` in the list
3. Press **F5** or click **Compile** button
4. Check for any compilation errors (there should be none)
5. Close the NinjaScript Editor

### Step 4: Add to Chart
1. Open a chart for the instrument you want to trade
2. Right-click on the chart
3. Go to **Add-On** → **TradeManagerPanel**
4. The panel should appear on the right side of your chart

## Configuration Guide

### Risk Management Settings

**Risk % of Account:**
- Enter the percentage of your account you want to risk per trade
- Default: 1.0
- Example: With $10,000 account and 1% risk = $100 risk per trade

**Stop Buffer (ticks):**
- Additional ticks added to the candlestick size for stop loss
- Default: 2
- Example: If last candle is 10 ticks and buffer is 2 ticks, total stop distance is 12 ticks

### Profit Targets

**Number of Targets:**
- Select 1, 2, or 3 profit targets from the dropdown
- Determines how many R multiples to use

**Target Percentages:**
- Enter the percentage of contracts to close at each target
- Must total 100% across all selected targets
- Examples:
  - 1 Target: 100%
  - 2 Targets: 50% / 50% or 60% / 40%
  - 3 Targets: 50% / 30% / 20% or 40% / 30% / 30%

### Auto Breakeven

**Enable Auto BE:**
- Check this box to enable automatic breakeven functionality

**Breakeven Mode:**
- **Fixed Ticks**: Uses a fixed number of ticks as trigger
- **1R + Buffer**: Uses risk distance (1R) as trigger

**BE Ticks (Mode 1):**
- Number of ticks price must move before breakeven triggers
- Only applies when "Fixed Ticks" mode is selected

**BE Buffer (ticks):**
- Additional ticks to add when moving stop to breakeven
- Applies to both modes
- Example: With 5 tick buffer, stop moves to entry price + 5 ticks instead of exact breakeven

## Usage Examples

### Example 1: Conservative Day Trading Setup
- **Risk**: 0.5% of account
- **Targets**: 3 targets
  - Target 1 (1R): 50% of contracts
  - Target 2 (2R): 30% of contracts
  - Target 3 (3R): 20% of contracts
- **Stop Buffer**: 3 ticks
- **Auto BE**: Enabled, Fixed Ticks mode, 8 ticks trigger, 2 tick buffer

### Example 2: Aggressive Swing Trading Setup
- **Risk**: 2% of account
- **Targets**: 2 targets
  - Target 1 (1R): 60% of contracts
  - Target 2 (2R): 40% of contracts
- **Stop Buffer**: 5 ticks
- **Auto BE**: Enabled, 1R + Buffer mode, 5 tick buffer

### Example 3: Scalping Setup
- **Risk**: 1% of account
- **Targets**: 1 target
  - Target 1 (1R): 100% of contracts
- **Stop Buffer**: 1 tick
- **Auto BE**: Enabled, Fixed Ticks mode, 5 ticks trigger, 1 tick buffer

## How It Works

### Trade Execution Flow

1. **User Configures Settings** in the panel
2. **User Clicks BUY or SELL** button
3. **Panel Reads Current Market Data**:
   - Gets current price
   - Analyzes last completed candlestick
4. **Calculates Stop Loss**:
   - Measures last candle size (High - Low)
   - Adds buffer ticks
   - Places stop accordingly (below entry for long, above for short)
5. **Calculates Profit Targets**:
   - Measures risk distance (R)
   - Calculates 1R = Entry + 1×R distance
   - Calculates 2R = Entry + 2×R distance
   - Calculates 3R = Entry + 3×R distance
6. **Calculates Position Size**:
   - Gets account balance
   - Calculates dollar risk (balance × risk %)
   - Calculates contracts (dollar risk ÷ risk per contract)
7. **Distributes Contracts** across selected targets based on percentages
8. **Displays Visual Projections** on chart
9. **Places All Orders**:
   - Market entry order
   - Stop loss order
   - Profit target orders
10. **Monitors for Breakeven** if enabled

## Technical Details

### Requirements
- NinjaTrader 8
- Active NinjaTrader account
- Real-time or simulated data feed

### Supported Order Types
- Entry: Market Order
- Stop Loss: Stop Market Order
- Profit Targets: Limit Orders

### Supported Instruments
- Futures
- Stocks
- Forex
- Any instrument supported by NinjaTrader 8

### Performance Considerations
- Panel is optimized for low CPU usage
- Visual updates are throttled to prevent lag
- Order execution is asynchronous

## Troubleshooting

### Panel Doesn't Appear
- Verify the script compiled without errors
- Check that you're adding it from Add-On menu (not Indicator menu)
- Restart NinjaTrader and try again

### Orders Not Placing
- Verify you have an active account connection
- Check that market is open
- Ensure you have sufficient buying power
- Verify instrument is tradeable in your account

### Visual Lines Not Showing
- Check that chart drawing is enabled
- Verify chart has sufficient bars loaded
- Try refreshing the chart (F5)

### Incorrect Position Sizing
- Verify risk percentage is entered correctly (1.0 for 1%, not 0.01)
- Check that account balance is correctly reported
- Ensure instrument point value is correct

### Breakeven Not Triggering
- Verify Auto BE checkbox is checked
- Check that selected mode is appropriate for market conditions
- Ensure trigger values are reasonable for instrument volatility

## Best Practices

1. **Test in Simulation First**: Always test with simulated trading before using real money
2. **Monitor First Trade**: Watch the first trade execute to verify all calculations are correct
3. **Set Reasonable Risk**: Never risk more than 1-2% of account per trade
4. **Adjust for Volatility**: Increase stop buffer during high volatility
5. **Use Appropriate Targets**: Ensure 1R, 2R, 3R targets are realistic for the instrument
6. **Review Target Percentages**: Make sure percentages add to 100%
7. **Test Breakeven Logic**: Verify breakeven triggers at expected levels
8. **Keep Buffer Reasonable**: Breakeven buffer should be at least 2-3 ticks to avoid whipsaw

## Safety Features

- Validates all user inputs before executing trades
- Prevents negative or zero contract orders
- Ensures target percentages sum correctly
- Provides clear status updates and error messages
- Implements fail-safes for missing market data

## Limitations

- Does not support OCO (One-Cancels-Other) bracket orders (managed separately)
- Requires active market data connection
- Visual projections update on new bar or manual refresh
- Breakeven monitoring requires price updates
- Does not support partial fills adjustment (assumes fills complete)

## Support and Updates

This is a complete, production-ready implementation. The code includes:
- Full error handling
- Comprehensive comments
- Modular design for easy modifications
- Industry-standard risk management practices

## License

This code is provided as-is for educational and trading purposes. Use at your own risk.

## Disclaimer

**TRADING INVOLVES RISK. PAST PERFORMANCE IS NOT INDICATIVE OF FUTURE RESULTS.**

This software is provided for educational purposes. The developer is not responsible for any trading losses incurred while using this tool. Always test thoroughly in simulation before trading live. Never risk more than you can afford to lose.

## Version History

**Version 1.0** - Initial Release
- Complete implementation of all features
- Visual projections
- Auto breakeven
- Position sizing
- Multi-target support
- Risk-based calculations

---

**Happy Trading! Trade Responsibly!**
