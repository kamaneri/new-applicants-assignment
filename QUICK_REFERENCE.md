# Trade Manager Panel - Quick Reference Guide

## Quick Start (5 Minutes)

### Installation
1. Download `TradeManagerPanel.cs`
2. Open NinjaTrader 8
3. Tools → Import → NinjaScript Add-On
4. Select the file and import
5. Tools → Edit NinjaScript → Add-On
6. Press F5 to compile
7. Right-click chart → Add-On → TradeManagerPanel

### Basic Usage
1. **Set your risk**: Enter risk percentage (e.g., 1.0 for 1%)
2. **Configure targets**: Select number of targets (1, 2, or 3)
3. **Enable Auto BE** (optional): Check box and select mode
4. **Click BUY or SELL**: Panel does everything automatically!

## Default Settings (Recommended for Beginners)

```
Risk % of Account:     1.0
Stop Buffer (ticks):   2
Number of Targets:     3
Target 1 %:           50
Target 2 %:           30
Target 3 %:           20
Enable Auto BE:       ✓ (checked)
Breakeven Mode:       1R + Buffer
BE Ticks (Mode 1):    10
BE Buffer (ticks):    5
```

## What Happens When You Click BUY/SELL?

1. ✅ Panel measures last candlestick size
2. ✅ Adds your buffer ticks to create stop loss
3. ✅ Calculates 1R, 2R, 3R profit targets
4. ✅ Calculates how many contracts based on your risk %
5. ✅ Splits contracts across your targets (50%/30%/20%)
6. ✅ Draws colored lines on your chart showing all levels
7. ✅ Places market entry order
8. ✅ Places stop loss order
9. ✅ Places profit target orders
10. ✅ Monitors for breakeven trigger (if enabled)

## Visual Guide (Chart Lines)

```
Orange Line    -------- Target 3 (3R) - Take 20% profit
Yellow Line    -------- Target 2 (2R) - Take 30% profit
Green Line     -------- Target 1 (1R) - Take 50% profit
White Line     -------- Entry Price
Blue Line*     -------- Breakeven Stop (after trigger)
Red Line       -------- Initial Stop Loss

* Blue line appears only after breakeven triggers
```

## Common Use Cases

### Day Trading ES (E-mini S&P 500)
```
Risk %:        0.5-1.0
Stop Buffer:   2-3 ticks
Targets:       3
BE Mode:       Fixed Ticks (8-10 ticks)
BE Buffer:     2 ticks
```

### Swing Trading NQ (E-mini NASDAQ)
```
Risk %:        1.0-2.0
Stop Buffer:   3-5 ticks
Targets:       2
BE Mode:       1R + Buffer
BE Buffer:     5 ticks
```

### Scalping MES (Micro E-mini S&P)
```
Risk %:        0.5
Stop Buffer:   1-2 ticks
Targets:       1
BE Mode:       Fixed Ticks (5 ticks)
BE Buffer:     1 tick
```

## Breakeven Modes Explained

### Mode 1: Fixed Ticks
- Simple and predictable
- Set ticks (e.g., 10)
- When price moves 10 ticks in your favor → stop moves to BE + buffer
- **Use when:** You want consistent breakeven trigger regardless of market conditions

### Mode 2: 1R + Buffer
- Dynamic based on your risk
- When price moves 1R (your stop distance) in your favor → stop moves to BE + buffer
- **Use when:** You want breakeven trigger proportional to your stop size
- **Example:** Stop is 20 ticks away, price moves 20 ticks in profit → BE triggers

## Target Percentage Examples

### Conservative (3 targets)
```
Target 1: 50% at 1R  ← Take half profit early
Target 2: 30% at 2R  ← Take most of remainder
Target 3: 20% at 3R  ← Let winners run
```

### Balanced (3 targets)
```
Target 1: 33% at 1R
Target 2: 33% at 2R
Target 3: 34% at 3R
```

### Aggressive (3 targets)
```
Target 1: 25% at 1R  ← Small early profit
Target 2: 25% at 2R
Target 3: 50% at 3R  ← Let half ride for big wins
```

### Two Targets Only
```
Target 1: 60% at 1R  ← Take majority early
Target 2: 40% at 2R  ← Let some run
```

### One Target (All or Nothing)
```
Target 1: 100% at 1R ← Quick in and out
```

## Troubleshooting

### "Error: Invalid input values"
- Check that all numbers are positive
- Ensure target percentages add to 100
- Risk % should be between 0.1 and 10

### "Error: Unable to get market data"
- Ensure chart has loaded price data
- Check data feed connection
- Try refreshing chart (F5)

### Orders not placing
- Verify account is connected
- Check market is open
- Ensure you have buying power
- Test in simulation first

### Lines not showing on chart
- Enable chart drawing (Tools → Options → Drawing)
- Ensure chart has enough bars loaded
- Try zooming out on chart

### Position size is 0 or 1 when you expected more
- Increase risk % slightly
- Your stop might be too wide for account size
- Example: $10k account, 1% risk = $100
  If stop is 10 points on ES ($50/point) = $500 risk per contract
  You need $500 to trade 1 contract at this stop distance
  Solution: Use 5% risk or tighten stop

## Safety Checklist

Before first live trade:
- [ ] Tested in simulation account (minimum 10 trades)
- [ ] Verified stop loss places correctly
- [ ] Verified targets place correctly
- [ ] Confirmed position size is reasonable
- [ ] Tested breakeven trigger (if using)
- [ ] Reviewed all settings match your strategy
- [ ] Have emergency stop plan (manual close if needed)
- [ ] Comfortable with maximum dollar risk per trade

## Formula Reference

### Stop Loss Distance
```
Candle Size = Last Candle High - Last Candle Low
Buffer = Stop Buffer Ticks × Tick Size
Stop Distance = Candle Size + Buffer
```

### R (Risk) Calculation
```
R = |Entry Price - Stop Loss Price|
```

### Profit Targets
```
For Long:
  Target 1 = Entry + (1 × R)
  Target 2 = Entry + (2 × R)
  Target 3 = Entry + (3 × R)

For Short:
  Target 1 = Entry - (1 × R)
  Target 2 = Entry - (2 × R)
  Target 3 = Entry - (3 × R)
```

### Position Size
```
Account Balance = Your Account Size
Dollar Risk = Balance × (Risk % ÷ 100)
Risk Per Contract = R × Point Value
Contracts = Dollar Risk ÷ Risk Per Contract (rounded down)
```

### Example Calculation
```
Account: $25,000
Risk: 1% = $250
Entry: 4500 (ES)
Last Candle: 10 points (4490-4500)
Buffer: 2 ticks (0.5 points)
Stop Distance: 10.5 points
Stop: 4500 - 10.5 = 4489.5

R = 10.5 points
Target 1: 4500 + 10.5 = 4510.5
Target 2: 4500 + 21.0 = 4521.0
Target 3: 4500 + 31.5 = 4531.5

Risk per contract = 10.5 × $50 = $525
Contracts = $250 ÷ $525 = 0.47 → 0 contracts (need more capital or less risk)

With 2% risk ($500):
Contracts = $500 ÷ $525 = 0.95 → 0 contracts (still not quite enough)

With $50k account at 1% risk ($500):
Contracts = $500 ÷ $525 = 0.95 → 0 contracts

Need to either:
- Increase account size
- Increase risk %
- Tighten stop (smaller R)
- Trade micro contracts (MES)
```

## Support

### Documentation Files
- `NINJATRADER_INSTALLATION.md` - Full installation guide
- `TECHNICAL_DOCUMENTATION.md` - Technical details and algorithms
- `TradeManagerPanel.cs` - Source code with comments

### Getting Help
1. Review error message in status label
2. Check this quick reference guide
3. Read full installation guide
4. Review technical documentation
5. Test in simulation to understand behavior

## Best Practices

1. **Always start with simulation** - Test thoroughly before live trading
2. **Risk small** - Never risk more than 1-2% per trade
3. **Adjust for volatility** - Increase stop buffer during high volatility
4. **Review each trade** - Make sure calculations look correct before entering
5. **Keep percentages simple** - Start with 50/30/20 split
6. **Use Auto BE** - Protect profits once in the money
7. **Monitor first few trades** - Watch how panel behaves with your account
8. **Match to your style** - Scalpers use 1 target, swingers use 3

## Quick Tips

💡 **Tip 1:** If position size is too small, either increase risk % or tighten your stop buffer

💡 **Tip 2:** Mode 2 Breakeven (1R + Buffer) is generally better than fixed ticks because it adapts to your risk

💡 **Tip 3:** For volatile instruments, increase stop buffer to avoid premature stops

💡 **Tip 4:** Start with default 50/30/20 target split - it's proven to work well

💡 **Tip 5:** Your first target (1R) should hit frequently (60-70% win rate on target 1 is healthy)

💡 **Tip 6:** If target 1 rarely hits, your entries might be poor OR stop buffer is too large

💡 **Tip 7:** During high impact news, disable Auto BE or increase buffer to avoid whipsaw

💡 **Tip 8:** Test panel with MES or MNQ (micro contracts) first - smaller risk while learning

---

## One-Page Cheat Sheet

```
┌─────────────────────────────────────────────────────────────┐
│                  TRADE MANAGER QUICK SETUP                  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Risk %:           1.0  ← 1% of account per trade          │
│  Stop Buffer:      2    ← Extra ticks for safety           │
│  Targets:          3    ← Use all 3 R multiples            │
│  Target % Split:   50/30/20  ← Take profit distribution    │
│  Auto BE:          ON   ← Move stop to breakeven           │
│  BE Mode:          1R+Buffer ← Adaptive trigger            │
│  BE Buffer:        5    ← Ticks above/below entry          │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                     HOW IT WORKS                            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. Measures last candle + buffer = Stop Distance          │
│  2. Calculates 1R, 2R, 3R targets from stop                │
│  3. Sizes position: Risk% × Account ÷ (R × Point Value)    │
│  4. Splits contracts: 50% @ 1R, 30% @ 2R, 20% @ 3R        │
│  5. Places orders: Entry, Stop, 3 Targets                  │
│  6. Monitors breakeven: When price = Entry + 1R            │
│  7. Moves stop to: Entry + Buffer when triggered           │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                    CHART COLORS                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🟠 Orange   = Target 3 (3R)                               │
│  🟡 Yellow   = Target 2 (2R)                               │
│  🟢 Green    = Target 1 (1R)                               │
│  ⚪ White    = Entry                                        │
│  🔵 Blue     = Breakeven Stop (when active)                │
│  🔴 Red      = Initial Stop Loss                           │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                 REMEMBER                                    │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ⚠️  TEST IN SIMULATION FIRST                              │
│  ⚠️  NEVER RISK MORE THAN 1-2% PER TRADE                   │
│  ⚠️  VERIFY CALCULATIONS BEFORE CLICKING BUY/SELL          │
│  ⚠️  HAVE A MANUAL EXIT PLAN IF NEEDED                     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

**Print this page and keep it next to your trading station!**

**Happy Trading! Manage Risk. Stay Disciplined. 📈**
