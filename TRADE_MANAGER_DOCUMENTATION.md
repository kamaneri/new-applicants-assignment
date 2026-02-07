# Trade Manager Panel for NinjaTrader 8

## Overview

The Trade Manager Panel is a comprehensive trading tool for NinjaTrader 8 that provides visual stop loss & profit target projection, automated risk-to-reward (R:R) targeting, position sizing based on account percentage, auto breakeven functionality, and one-click trade execution.

## Features

### 1. Visual Stop Loss & Profit Target Projection
- Displays stop loss level visually on the chart (red line)
- Shows up to 4 profit targets (green lines for active targets, yellow dashed line for 4R reference)
- All levels update dynamically with the current chart price
- Visual lines adjust automatically when chart is scrolled or zoomed

### 2. Automatic Stop Loss Calculation
- Calculates stop loss based on:
  - The size of the most recent candlestick (High - Low)
  - Plus a user-defined tick buffer
- Formula: `Stop Loss Distance = Candle Range + (Tick Buffer × Tick Size)`

### 3. Automated Risk to Reward (R:R) Targeting
- Automatically calculates and places profit targets:
  - **Target 1 (1R)**: Distance equal to stop loss
  - **Target 2 (2R)**: Distance equal to 2× stop loss
  - **Target 3 (3R)**: Distance equal to 3× stop loss
  - **4R Reference**: Visual reference line (not an actual order)
- Each target allows predefined percentage allocation of total contracts
- Contract distribution is customizable per target

### 4. Position Sizing Based on Account Percentage
- Risk a fixed percentage of account balance (default: 1%)
- System automatically:
  - Calculates the number of contracts based on stop loss distance and risk percentage
  - Distributes contracts across selected profit targets according to predefined percentages
- Formula: `Total Contracts = (Account Balance × Risk%) / (Stop Loss Distance in Ticks × Tick Value)`

### 5. Profit Target Toggle Options
- Choose between:
  - **1 Target**: 100% of contracts at 1R
  - **2 Targets**: Distributed between 1R and 2R
  - **3 Targets**: Distributed between 1R, 2R, and 3R
- Contract distribution automatically adjusts based on selected number of targets
- Default distribution: 50% at 1R, 30% at 2R, 20% at 3R

### 6. Auto Breakeven Function
Two modes available:

#### Mode 1 — Fixed Tick Trigger
- Breakeven activates when price moves a user-defined number of ticks in favor
- Example: If set to 10 ticks, stop moves to breakeven when profit reaches 10 ticks

#### Mode 2 — Risk Based Trigger (1R)
- Breakeven activates when price moves 1R in favor (equal to stop loss distance)
- Additional user-defined tick buffer applied after breakeven trigger
- Example: If stop loss is 50 ticks, price moves 50 ticks in favor, and buffer is +5 ticks:
  - Stop moves to entry price + 5 ticks (instead of just breakeven)
- Buffer is fully configurable (default: 2 ticks)

### 7. Execution Buttons
- **BUY Button** (Green): Execute long trade
- **SELL Button** (Red): Execute short trade
- Both buttons automatically:
  - Apply all stop loss calculations
  - Place all profit targets
  - Apply R:R calculations
  - Enable breakeven logic
  - Calculate and execute proper contract sizing

## Installation

1. **Locate NinjaTrader 8 Custom Folder**:
   - Open NinjaTrader 8
   - Go to: `Tools` → `Edit NinjaScript` → `Indicator`
   - This opens the indicators folder

2. **Copy File**:
   - Copy `TradeManagerPanel.cs` to the indicators folder
   - Default location: `Documents\NinjaTrader 8\bin\Custom\Indicators\`

3. **Compile**:
   - In NinjaTrader, press `F5` or go to `Tools` → `Compile NinjaScript`
   - Check for compilation errors in the output window
   - If successful, you'll see "Compiled successfully"

4. **Add to Chart**:
   - Right-click on any chart
   - Select `Indicators` → `TradeManagerPanel`
   - Click `OK` to apply

## Configuration

### Risk Management Settings

| Property | Description | Default | Range |
|----------|-------------|---------|-------|
| Account Risk % | Percentage of account to risk per trade | 1.0% | 0.1 - 10.0% |
| Stop Loss Tick Buffer | Additional ticks beyond candlestick range | 2 | 1 - 100 |

### Profit Target Settings

| Property | Description | Default | Range |
|----------|-------------|---------|-------|
| Number of Targets | How many profit targets to use | 3 | 1 - 3 |
| Target 1 % | Percentage of contracts for Target 1 (1R) | 50% | 1 - 100% |
| Target 2 % | Percentage of contracts for Target 2 (2R) | 30% | 1 - 100% |
| Target 3 % | Percentage of contracts for Target 3 (3R) | 20% | 1 - 100% |

**Note**: Target percentages must sum to 100% for active targets.

### Auto Breakeven Settings

| Property | Description | Default | Range |
|----------|-------------|---------|-------|
| Enable Auto Breakeven | Enable automatic breakeven functionality | True | True/False |
| Breakeven Mode | Fixed Ticks or Risk Based (1R) | Risk Based | - |
| BE Fixed Ticks | Ticks in profit to trigger BE (Fixed Mode) | 10 | 1 - 200 |
| BE Tick Buffer | Additional ticks beyond breakeven | 2 | 0 - 50 |

### Display Settings

| Property | Description | Default | Range |
|----------|-------------|---------|-------|
| Show Visual Levels | Display stop loss and target lines | True | True/False |
| Panel X Position | Horizontal position of panel | 10 | 0 - 2000 |
| Panel Y Position | Vertical position of panel | 100 | 0 - 2000 |

## Usage Guide

### Basic Trading Workflow

1. **Add Indicator to Chart**:
   - Apply TradeManagerPanel to your trading chart
   - The panel appears on the left side of the chart

2. **Configure Settings** (Optional):
   - Adjust Account Risk % (how much you want to risk)
   - Set Stop Loss Tick Buffer (additional safety margin)
   - Configure Target distribution percentages
   - Enable/disable Auto Breakeven and set mode

3. **Execute Trade**:
   - Click **BUY** for a long position
   - Click **SELL** for a short position
   - Panel automatically:
     - Calculates stop loss based on current candle
     - Calculates 1R, 2R, 3R targets
     - Determines position size
     - Places all orders
     - Displays visual levels on chart

4. **Monitor Trade**:
   - Visual lines show stop loss (red) and targets (green)
   - Auto breakeven activates automatically when triggered
   - Profit targets are hit sequentially

5. **Trade Management**:
   - Stop loss automatically moves to breakeven when configured conditions are met
   - Profit targets are filled as price reaches each level
   - Remaining contracts close at final target or stop loss

### Example Trade Scenario

**Setup**:
- Account Balance: $10,000
- Account Risk: 1% = $100
- Current Candle Range: 20 ticks
- Tick Buffer: 2 ticks
- Stop Loss Distance: 22 ticks (20 + 2)
- Tick Value: $12.50
- Number of Targets: 3
- Distribution: 50% / 30% / 20%
- Auto BE Mode: Risk Based (1R)
- BE Buffer: 2 ticks

**Execution** (Click BUY button):
1. Entry Price: 4,500.00
2. Stop Loss: 4,500.00 - 22 ticks = 4,472.50
3. Position Size: $100 / (22 × $12.50) = 0.36 → 1 contract (minimum)
   - (In practice with larger account, would be multiple contracts)
4. Targets:
   - Target 1 (1R): 4,522.50 (50% of contracts)
   - Target 2 (2R): 4,545.00 (30% of contracts)
   - Target 3 (3R): 4,567.50 (20% of contracts)
5. BE Trigger: 4,522.50 (1R distance)
6. BE Stop Price: 4,502.50 (entry + 2 tick buffer)

**Trade Progression**:
- Price reaches 4,522.50 → Target 1 hit (50% closed), Stop moves to 4,502.50 (BE + buffer)
- Price reaches 4,545.00 → Target 2 hit (30% closed)
- Price reaches 4,567.50 → Target 3 hit (20% closed), trade complete

## Code Structure

### Main Components

1. **UI Panel Creation** (`CreateWPFControls()`):
   - Creates WPF grid with all controls
   - Text boxes for configuration
   - Combo boxes for dropdowns
   - Buy/Sell buttons

2. **Trade Execution** (`ExecuteTrade()`):
   - Validates inputs
   - Calculates stop loss and targets
   - Determines position size
   - Places all orders

3. **Stop Loss Calculation** (`CalculateStopLoss()`):
   - Gets current candle range (High - Low)
   - Adds tick buffer
   - Calculates final stop price

4. **Target Calculation** (`CalculateTargets()`):
   - Calculates 1R, 2R, 3R, 4R levels
   - Rounds to proper tick size

5. **Position Sizing** (`CalculatePositionSize()`):
   - Gets account balance
   - Calculates risk amount in dollars
   - Determines contracts based on risk per contract

6. **Breakeven Logic** (`CheckBreakevenTrigger()`, `MoveStopToBreakeven()`):
   - Monitors price relative to trigger level
   - Moves stop when conditions are met
   - Applies buffer beyond breakeven

7. **Visual Levels** (`DrawVisualLevels()`, `UpdateVisualLevels()`):
   - Draws horizontal lines on chart
   - Updates positions as chart scrolls/zooms
   - Color-coded: Red (stop), Green (targets), Yellow (4R reference)

## Requirements

- **NinjaTrader 8**: Version 8.0 or later
- **.NET Framework**: 4.8 or later (included with NinjaTrader)
- **Account Type**: Live or Simulation account
- **Connection**: Active data feed connection
- **Permissions**: Adequate trading permissions and account balance

## Best Practices

1. **Start with Simulation**:
   - Test the panel thoroughly in simulation mode
   - Verify calculations are correct for your instrument
   - Practice using all features

2. **Verify Calculations**:
   - Check that stop loss distance makes sense
   - Verify position size is appropriate for your account
   - Ensure target percentages sum to 100%

3. **Risk Management**:
   - Start with conservative risk % (0.5% - 1%)
   - Never risk more than you can afford to lose
   - Understand tick values for your instrument

4. **Monitor Trades**:
   - Always monitor active trades
   - Panel automates execution but doesn't replace monitoring
   - Be ready to manually intervene if needed

5. **Regular Review**:
   - Review trade results regularly
   - Adjust target distribution based on performance
   - Fine-tune breakeven settings for your strategy

## Troubleshooting

### Panel Not Appearing
- Check that indicator compiled successfully (F5)
- Verify panel X/Y positions are within chart boundaries
- Try adjusting Panel X and Panel Y properties

### Orders Not Placing
- Verify account is connected and active
- Check that sufficient account balance exists
- Ensure instrument has adequate liquidity
- Review NinjaTrader Output window for errors

### Visual Lines Not Showing
- Verify "Show Visual Levels" is enabled
- Check that a trade has been executed
- Try refreshing the chart (F5)

### Position Size Too Small/Large
- Adjust Account Risk % setting
- Verify account balance in account settings
- Check instrument tick value and point value
- Review calculation in NinjaTrader Output window

### Breakeven Not Triggering
- Verify "Enable Auto Breakeven" is checked
- Check that price has reached trigger level
- Ensure stop order is still working
- Review trigger price in Output window

## Support and Contact

For issues, questions, or feature requests related to this Trade Manager Panel:

1. Check NinjaTrader Output window for error messages
2. Review this documentation thoroughly
3. Test in simulation mode first
4. Contact support with detailed description of issue

## Disclaimer

**IMPORTANT**: This software is provided for educational and informational purposes. Trading futures, forex, and equities involves substantial risk of loss and is not suitable for all investors. Past performance is not indicative of future results. The creators of this panel are not responsible for any losses incurred through its use. Always test thoroughly in simulation before using with real money. Consult with a qualified financial advisor before trading.

## Version History

### Version 1.0 (2026-02-07)
- Initial release
- Full feature implementation as specified
- Visual stop loss and profit target projection
- Automated R:R targeting (1R, 2R, 3R, 4R)
- Position sizing based on account percentage
- Profit target toggle (1-3 targets)
- Auto breakeven with two modes (Fixed Ticks, Risk Based)
- Buy/Sell execution buttons
- Complete NinjaTrader 8 compatibility

## License

This code is provided as-is for use with NinjaTrader 8. Users are free to modify and adapt the code for their personal trading needs.
