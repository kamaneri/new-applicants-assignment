# Trade Manager Panel for NinjaTrader 8

A professional-grade Trade Manager Panel for NinjaTrader 8 that provides visual stop loss & profit target projection, automated risk-to-reward targeting, intelligent position sizing, auto breakeven functionality, and one-click trade execution.

## Overview

This repository contains a comprehensive trading tool designed to automate risk management and trade execution in NinjaTrader 8. The panel calculates optimal stop loss levels based on market volatility, sizes positions according to your risk tolerance, and manages profit targets using risk-to-reward multiples (R:R).

## Key Features

- **Visual Stop Loss & Profit Target Projection** - See all your levels on the chart in real-time
- **Automatic Stop Loss Calculation** - Based on candlestick range plus configurable tick buffer
- **Automated R:R Targeting** - 1R, 2R, 3R profit targets with customizable contract distribution
- **Smart Position Sizing** - Risk fixed percentage of account balance
- **Auto Breakeven** - Two modes: Fixed tick trigger or Risk-based (1R) trigger
- **One-Click Execution** - Buy/Sell buttons place entire trade setup automatically
- **Contract Distribution** - Customize allocation across multiple profit targets

## Quick Start

### Installation

1. **Download** the `TradeManagerPanel.cs` file from this repository
2. **Open NinjaTrader 8** and go to: `Tools` → `Edit NinjaScript` → `Indicator`
3. **Copy** `TradeManagerPanel.cs` to the indicators folder that opens
4. **Compile** by pressing `F5` (or `Tools` → `Compile NinjaScript`)
5. **Add to Chart**: Right-click chart → `Indicators` → `TradeManagerPanel` → `OK`

### Basic Usage

1. Configure your risk percentage (default: 1%)
2. Set number of profit targets (1-3)
3. Click **BUY** or **SELL** button
4. Panel automatically calculates and places all orders
5. Visual lines display on chart showing stop loss and targets

See **[TRADE_MANAGER_QUICKSTART.md](TRADE_MANAGER_QUICKSTART.md)** for detailed quick start guide.

## Documentation

- **[TRADE_MANAGER_QUICKSTART.md](TRADE_MANAGER_QUICKSTART.md)** - Quick start and installation guide
- **[TRADE_MANAGER_DOCUMENTATION.md](TRADE_MANAGER_DOCUMENTATION.md)** - Complete documentation with examples and troubleshooting

## Requirements

- NinjaTrader 8 (any version)
- .NET Framework 4.8+ (included with NinjaTrader)
- Live or Simulation trading account
- Active data feed connection

## Features in Detail

### 1. Visual Stop Loss & Profit Target Projection
- Real-time display of stop loss (red line) and profit targets (green lines)
- Updates dynamically as chart moves
- Visual 4R reference line (yellow dashed)

### 2. Automatic Stop Loss Calculation  
- Uses most recent candlestick range (High - Low)
- Adds configurable tick buffer for safety margin
- Formula: `Stop Distance = Candle Range + Tick Buffer`

### 3. Automated R:R Targeting
- **1R**: Distance equal to stop loss
- **2R**: Distance equal to 2× stop loss  
- **3R**: Distance equal to 3× stop loss
- Contract allocation customizable per target

### 4. Position Sizing Based on Account Percentage
- Risk fixed % of account per trade (e.g., 1%)
- Automatically calculates number of contracts
- Distributes contracts across selected targets

### 5. Profit Target Toggle Options
- Choose 1, 2, or 3 profit targets
- Default distribution: 50% / 30% / 20%
- Fully customizable allocation

### 6. Auto Breakeven Function

**Mode 1 - Fixed Tick Trigger**
- Moves stop to breakeven after X ticks in profit
- Configurable tick buffer beyond breakeven

**Mode 2 - Risk Based Trigger (1R)**  
- Moves stop to breakeven after 1R profit (equal to stop loss distance)
- Configurable tick buffer beyond breakeven
- Example: 50 tick stop + 5 tick buffer = stop at entry + 5 ticks

### 7. Execution Buttons
- **BUY** button (green) for long positions
- **SELL** button (red) for short positions  
- One click places entire trade setup:
  - Entry order
  - Stop loss order
  - All profit target orders
  - Enables breakeven monitoring

## Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| Account Risk % | Percentage of account to risk | 1.0% |
| Stop Loss Tick Buffer | Additional ticks for stop | 2 |
| Number of Targets | Profit targets to use (1-3) | 3 |
| Target 1 % | Contracts for 1R target | 50% |
| Target 2 % | Contracts for 2R target | 30% |
| Target 3 % | Contracts for 3R target | 20% |
| Enable Auto Breakeven | Turn on/off auto BE | On |
| Breakeven Mode | Fixed Ticks or Risk Based | Risk Based |
| BE Fixed Ticks | Ticks for fixed mode trigger | 10 |
| BE Tick Buffer | Buffer beyond breakeven | 2 |
| Show Visual Levels | Display lines on chart | On |
| Panel X/Y Position | Panel location on chart | 10, 100 |

## Example Trade Workflow

**Setup**: ES Futures, $10,000 account, 1% risk, 3 targets (50%/30%/20%)

1. Current candle has 20 tick range
2. Click **BUY** at 4,500.00
3. Panel calculates:
   - Stop Loss: 4,500 - 22 ticks = 4,472.50 (20 + 2 buffer)
   - Target 1 (1R): 4,522.50 (50% contracts)
   - Target 2 (2R): 4,545.00 (30% contracts)
   - Target 3 (3R): 4,567.50 (20% contracts)
   - Position size: Based on $100 risk at 22 tick stop
   - BE Trigger: 4,522.50 (1R)
4. All orders placed automatically
5. Visual lines appear on chart
6. At 4,522.50: Target 1 fills, stop moves to 4,502.50 (entry + 2 tick buffer)
7. At 4,545.00: Target 2 fills
8. At 4,567.50: Target 3 fills, trade complete

## Important Notes

⚠️ **Always Test in Simulation First**
- Practice with all features in sim mode
- Verify calculations for your specific instrument
- Understand behavior before live trading

⚠️ **Risk Disclaimer**  
Trading involves substantial risk of loss. This tool does not guarantee profits. Past performance is not indicative of future results. Always use proper risk management and never risk more than you can afford to lose.

## Troubleshooting

**Panel not visible?**
- Check Panel X/Y position settings
- Verify successful compilation (F5)
- Try adjusting position values

**Orders not placing?**
- Verify account connection
- Check account balance sufficient
- Review NinjaTrader Output window for errors

**Position size incorrect?**
- Verify Account Risk % setting
- Check instrument tick value
- Review calculation in Output window

**Breakeven not triggering?**
- Ensure "Enable Auto Breakeven" is checked
- Verify price reached trigger level
- Check stop order is still working

## Files

- **TradeManagerPanel.cs** - Main indicator file for NinjaTrader 8
- **TRADE_MANAGER_DOCUMENTATION.md** - Complete documentation  
- **TRADE_MANAGER_QUICKSTART.md** - Quick start guide
- **README.md** - This file

## Support

For issues or questions:
1. Review the documentation files
2. Check NinjaTrader Output window for error messages
3. Test in simulation mode first
4. Ensure all settings are valid

## License

This code is provided as-is for use with NinjaTrader 8. Users are free to modify and adapt for personal trading needs.

## Credits

Developed as a comprehensive trade management solution for NinjaTrader 8, implementing professional-grade risk management and automated trade execution.

---

**Ready to enhance your trading? Install the Trade Manager Panel and start testing in simulation today!**
