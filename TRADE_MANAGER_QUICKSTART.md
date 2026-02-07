# Trade Manager Panel - Quick Start Guide

## What Is This?

This repository contains a **Trade Manager Panel for NinjaTrader 8** - a professional trading tool that automates risk management, position sizing, and trade execution.

## Key Features

✅ **Visual Stop Loss & Targets** - See your stop and profit levels on the chart  
✅ **Automatic Risk Calculation** - Stop loss based on candle size + buffer  
✅ **R:R Targeting** - Automated 1R, 2R, 3R profit targets  
✅ **Smart Position Sizing** - Risk fixed % of account per trade  
✅ **Auto Breakeven** - Two modes: Fixed ticks or Risk-based (1R)  
✅ **One-Click Execution** - Buy/Sell buttons place entire trade setup  
✅ **Contract Distribution** - Customize % allocation across targets  

## Quick Installation

### Step 1: Download the File
Download `TradeManagerPanel.cs` from this repository.

### Step 2: Install in NinjaTrader
1. Open **NinjaTrader 8**
2. Go to: **Tools** → **Edit NinjaScript** → **Indicator**
3. Copy `TradeManagerPanel.cs` into the indicators folder that opens
4. Press **F5** to compile (or **Tools** → **Compile NinjaScript**)

### Step 3: Add to Chart
1. Right-click on any chart
2. Select **Indicators** → **TradeManagerPanel**
3. Configure settings (or use defaults)
4. Click **OK**

The panel will appear on your chart!

## Quick Usage

1. **Set Your Risk**: Enter your desired account risk % (default: 1%)
2. **Configure Targets**: Choose 1, 2, or 3 profit targets
3. **Click BUY or SELL**: Panel automatically:
   - Calculates stop loss from current candle
   - Calculates 1R, 2R, 3R targets
   - Sizes position based on your risk
   - Places all orders
   - Shows visual lines on chart

That's it! Your trade is fully managed.

## Default Settings

| Setting | Default Value |
|---------|--------------|
| Account Risk | 1% |
| Stop Loss Buffer | 2 ticks |
| Number of Targets | 3 |
| Target Distribution | 50% / 30% / 20% |
| Auto Breakeven | Enabled (Risk Based) |
| Breakeven Buffer | 2 ticks |

## Example Trade

**Scenario**: ES Futures, Account = $10,000, Risk = 1%

1. Current candle: 20 ticks range
2. Click **BUY**
3. Panel calculates:
   - Stop: 22 ticks below entry (20 + 2 buffer)
   - Target 1: 22 ticks above (1R)
   - Target 2: 44 ticks above (2R)
   - Target 3: 66 ticks above (3R)
   - Position size: Based on $100 risk (1% of $10k)
4. All orders placed automatically
5. Visual lines show levels on chart
6. When profit reaches 1R, stop moves to breakeven + 2 ticks

## Documentation

For complete documentation, see:
- **[TRADE_MANAGER_DOCUMENTATION.md](TRADE_MANAGER_DOCUMENTATION.md)** - Full features, configuration, examples

## Files in This Repository

- **TradeManagerPanel.cs** - Main NinjaTrader 8 indicator file
- **TRADE_MANAGER_DOCUMENTATION.md** - Complete documentation
- **TRADE_MANAGER_QUICKSTART.md** - This file

## System Requirements

- NinjaTrader 8 (any version)
- .NET Framework 4.8+ (included with NinjaTrader)
- Live or Simulation account
- Active data feed

## Important Notes

⚠️ **Test in Simulation First!**
- Always test new tools in simulation mode
- Verify calculations are correct for your instrument
- Practice using all features

⚠️ **Risk Disclaimer**
- Trading involves substantial risk of loss
- This tool does not guarantee profits
- Always use proper risk management
- Test thoroughly before live trading

## Support

If you encounter issues:
1. Check NinjaTrader's **Output** window for error messages
2. Verify compilation was successful (no errors after pressing F5)
3. Ensure all settings are valid (e.g., target % sum to 100%)
4. Review the full documentation

## Panel Controls

The panel includes:

**Risk Management**
- Account Risk % input
- Stop Loss Tick Buffer input

**Profit Targets**
- Number of Targets dropdown (1, 2, or 3)
- Target 1, 2, 3 percentage inputs

**Auto Breakeven**
- Enable checkbox
- Mode dropdown (Fixed Ticks or Risk Based)
- BE Ticks input (for Fixed mode)
- BE Buffer input

**Execution**
- **BUY** button (green)
- **SELL** button (red)

## Visual Elements

When a trade is active, you'll see:
- **Red line** - Stop loss level
- **Green lines** - Active profit targets (1R, 2R, 3R)
- **Yellow dashed line** - 4R reference (not an order, just visual)

Lines update automatically as the chart scrolls or zooms.

## Getting Started Tips

1. **Start Small**: Use 0.5% - 1% risk initially
2. **Verify Math**: Check that position sizing makes sense
3. **Watch First Trades**: Monitor carefully to understand behavior
4. **Adjust Settings**: Fine-tune target distribution and BE settings
5. **Keep It Simple**: Use default settings until comfortable

## Common Questions

**Q: Why aren't my orders placing?**
A: Check account connection, balance, and review Output window for errors.

**Q: Position size seems wrong?**
A: Verify Account Risk % and check instrument's tick value/point value.

**Q: Breakeven not triggering?**
A: Ensure it's enabled and price has reached the trigger level.

**Q: Can I manually close parts of the position?**
A: Yes, but this may affect the panel's auto-management.

**Q: Does this work on any instrument?**
A: Yes, it works with any instrument in NinjaTrader (futures, forex, stocks).

## Credits

Created for NinjaTrader 8 as a complete trade management solution.

---

**Ready to trade smarter? Install the panel and start testing in simulation today!**
