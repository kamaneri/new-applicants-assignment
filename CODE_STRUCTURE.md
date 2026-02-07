# Trade Manager Panel - Code Structure

## File Organization

```
TradeManagerPanel.cs
├── Using Declarations
│   ├── System libraries
│   ├── WPF libraries
│   └── NinjaTrader libraries
│
├── Namespace: NinjaTrader.NinjaScript.Indicators
│
└── Class: TradeManagerPanel (Indicator)
    │
    ├── Variables
    │   ├── UI Components (Grid, Buttons, TextBoxes, ComboBoxes, CheckBoxes)
    │   ├── Visual Elements (Lines for stop loss and targets)
    │   ├── Chart Components (ChartControl, ChartScale)
    │   └── Trade Variables (prices, contracts, orders)
    │
    ├── Properties (User Configurable)
    │   ├── Risk Management (AccountRiskPercent, StopLossTickBuffer)
    │   ├── Targets (NumberOfTargets, Target1/2/3 Percent)
    │   ├── Breakeven (EnableAutoBreakeven, BEMode, BreakevenTicks, BreakevenBuffer)
    │   └── Display (ShowVisualLevels, PanelX, PanelY)
    │
    ├── Enums
    │   └── BreakevenMode (FixedTicks, RiskBased)
    │
    ├── NinjaScript Methods
    │   ├── OnStateChange()
    │   │   ├── State.SetDefaults - Initialize default values
    │   │   ├── State.Historical - Create WPF controls
    │   │   └── State.Terminated - Dispose WPF controls
    │   │
    │   └── OnBarUpdate()
    │       ├── Update visual levels
    │       └── Check breakeven trigger
    │
    ├── WPF UI Creation
    │   ├── CreateWPFControls() - Build panel interface
    │   ├── AddLabel() - Helper for adding labels
    │   ├── AddSeparator() - Helper for adding separators
    │   └── DisposeWPFControls() - Cleanup
    │
    ├── Event Handlers
    │   ├── TargetCountCombo_SelectionChanged()
    │   ├── BuyButton_Click()
    │   └── SellButton_Click()
    │
    ├── Trade Execution
    │   ├── ExecuteTrade() - Main execution logic
    │   ├── UpdatePropertiesFromUI() - Get current UI values
    │   ├── ValidateInputs() - Verify settings are valid
    │   ├── CalculateStopLoss() - Determine stop price
    │   ├── CalculateTargets() - Calculate 1R, 2R, 3R, 4R
    │   ├── CalculatePositionSize() - Determine contracts
    │   └── PlaceTargetOrders() - Submit target orders
    │
    ├── Breakeven Logic
    │   ├── CheckBreakevenTrigger() - Monitor for trigger
    │   └── MoveStopToBreakeven() - Adjust stop price
    │
    └── Visual Levels
        ├── DrawVisualLevels() - Create lines on chart
        ├── CreateLine() - Helper for line creation
        ├── UpdateVisualLevels() - Refresh line positions
        ├── UpdateLine() - Helper for line updates
        └── RemoveVisualLevels() - Cleanup lines
```

## Execution Flow

### Trade Entry Flow
```
User clicks BUY/SELL
    ↓
ExecuteTrade(isLong)
    ↓
UpdatePropertiesFromUI() - Get current settings
    ↓
ValidateInputs() - Check if settings are valid
    ↓
CalculateStopLoss() - Use candle range + buffer
    ↓
CalculateTargets() - Calculate 1R, 2R, 3R, 4R
    ↓
CalculatePositionSize() - Risk % of account
    ↓
Place Entry Order (EnterLong/EnterShort)
    ↓
Place Stop Loss Order (ExitLongStopMarket/ExitShortStopMarket)
    ↓
PlaceTargetOrders() - Place 1-3 target orders
    ↓
Calculate Breakeven Trigger Price
    ↓
DrawVisualLevels() - Show lines on chart
    ↓
Trade Active
```

### Breakeven Flow (Per Bar)
```
OnBarUpdate()
    ↓
Is Auto Breakeven Enabled?
    ↓ YES
Is Position Open?
    ↓ YES
Has Breakeven Already Triggered?
    ↓ NO
CheckBreakevenTrigger()
    ↓
Mode: Fixed Ticks
    → Price moved X ticks in profit?
Mode: Risk Based
    → Price moved 1R in profit?
    ↓ YES
MoveStopToBreakeven()
    ↓
New Stop = Entry + Buffer (Long)
New Stop = Entry - Buffer (Short)
    ↓
ChangeOrder() - Modify stop order
    ↓
Set breakEvenTriggered = true
```

## Key Calculations

### Stop Loss Distance
```
candleRange = High[0] - Low[0]
bufferAmount = StopLossTickBuffer × TickSize
stopLossDistance = candleRange + bufferAmount

For LONG:
  stopLossPrice = entryPrice - stopLossDistance
  
For SHORT:
  stopLossPrice = entryPrice + stopLossDistance
```

### Target Prices
```
For LONG:
  target1Price = entryPrice + (stopLossDistance × 1)  // 1R
  target2Price = entryPrice + (stopLossDistance × 2)  // 2R
  target3Price = entryPrice + (stopLossDistance × 3)  // 3R
  target4Price = entryPrice + (stopLossDistance × 4)  // 4R (visual only)

For SHORT:
  target1Price = entryPrice - (stopLossDistance × 1)  // 1R
  target2Price = entryPrice - (stopLossDistance × 2)  // 2R
  target3Price = entryPrice - (stopLossDistance × 3)  // 3R
  target4Price = entryPrice - (stopLossDistance × 4)  // 4R (visual only)
```

### Position Sizing
```
accountBalance = Account.Get(AccountItem.CashValue, Currency.UsDollar)
riskAmount = accountBalance × (AccountRiskPercent / 100)
stopLossTicks = |stopLossDistance / TickSize|
riskPerContract = stopLossTicks × TickSize × PointValue
totalContracts = floor(riskAmount / riskPerContract)
```

### Contract Distribution
```
target1Contracts = round(totalContracts × Target1Percent / 100)
target2Contracts = round(totalContracts × Target2Percent / 100)
target3Contracts = totalContracts - target1Contracts - target2Contracts

// Adjusted based on NumberOfTargets:
If NumberOfTargets = 1:
  target1Contracts = totalContracts
  target2Contracts = 0
  target3Contracts = 0

If NumberOfTargets = 2:
  target2Contracts = totalContracts - target1Contracts
  target3Contracts = 0

If NumberOfTargets = 3:
  (use calculated distribution)
```

### Breakeven Trigger Price
```
Mode 1 - Fixed Ticks:
  For LONG:
    breakEvenTriggerPrice = entryPrice + (BreakevenTicks × TickSize)
  For SHORT:
    breakEvenTriggerPrice = entryPrice - (BreakevenTicks × TickSize)

Mode 2 - Risk Based (1R):
  For LONG:
    breakEvenTriggerPrice = entryPrice + stopLossDistance
  For SHORT:
    breakEvenTriggerPrice = entryPrice - stopLossDistance
```

### Breakeven Stop Price
```
For LONG:
  newStopPrice = entryPrice + (BreakevenBuffer × TickSize)

For SHORT:
  newStopPrice = entryPrice - (BreakevenBuffer × TickSize)
```

## UI Panel Structure

```
┌─────────────────────────────────────┐
│      TRADE MANAGER                  │
├─────────────────────────────────────┤
│  Risk Management                    │
│  ┌─────────────────────────────┐   │
│  │ Account Risk %:      [1.00] │   │
│  │ SL Tick Buffer:      [2]    │   │
│  └─────────────────────────────┘   │
├─────────────────────────────────────┤
│  Profit Targets                     │
│  ┌─────────────────────────────┐   │
│  │ Number of Targets: [3 Targets]│ │
│  │ Target 1 % (1R):   [50]     │   │
│  │ Target 2 % (2R):   [30]     │   │
│  │ Target 3 % (3R):   [20]     │   │
│  └─────────────────────────────┘   │
├─────────────────────────────────────┤
│  Auto Breakeven                     │
│  ┌─────────────────────────────┐   │
│  │ [✓] Enable Auto Breakeven   │   │
│  │ BE Mode: [Risk Based (1R)]  │   │
│  │ BE Ticks (Fixed):    [10]   │   │
│  │ BE Buffer:           [2]    │   │
│  └─────────────────────────────┘   │
├─────────────────────────────────────┤
│  Execute Trade                      │
│  ┌─────────────────────────────┐   │
│  │         [ BUY ]             │   │ (Green)
│  │        [ SELL ]             │   │ (Red)
│  └─────────────────────────────┘   │
└─────────────────────────────────────┘
```

## Visual Levels on Chart

```
Price
  ↑
  │ -------- Target 4 (4R) [Yellow Dashed - Reference Only]
  │
  │ -------- Target 3 (3R) [Green - 20% contracts]
  │
  │ -------- Target 2 (2R) [Green - 30% contracts]
  │
  │ -------- Target 1 (1R) [Green - 50% contracts]
  │
  │ ======== Entry Price
  │
  │ -------- Stop Loss [Red - 100% contracts]
  │
  ↓
Time →
```

## Properties Configuration Window

When you add the indicator to a chart, NinjaTrader shows:

```
┌─────────────────────────────────────────┐
│  TradeManagerPanel Properties           │
├─────────────────────────────────────────┤
│  Risk Management                        │
│    Account Risk %:           [1.0    ]  │
│    Stop Loss Tick Buffer:    [2      ]  │
│                                         │
│  Targets                                │
│    Number of Targets:        [3      ]  │
│    Target 1 %:               [50     ]  │
│    Target 2 %:               [30     ]  │
│    Target 3 %:               [20     ]  │
│                                         │
│  Breakeven                              │
│    Enable Auto Breakeven:    [✓]        │
│    Breakeven Mode:           [Risk Based]│
│    BE Fixed Ticks:           [10     ]  │
│    BE Tick Buffer:           [2      ]  │
│                                         │
│  Display                                │
│    Show Visual Levels:       [✓]        │
│    Panel X Position:         [10     ]  │
│    Panel Y Position:         [100    ]  │
│                                         │
│         [ OK ]        [ Cancel ]        │
└─────────────────────────────────────────┘
```

## Order Placement Summary

When BUY button is clicked with 10 contracts, 3 targets at 50%/30%/20%:

```
Orders Placed:
1. Entry Order
   - Type: Market
   - Action: Buy
   - Quantity: 10
   - Signal Name: "Entry"

2. Stop Loss Order
   - Type: Stop Market
   - Action: Sell
   - Quantity: 10
   - Stop Price: Calculated stop loss price
   - Signal Name: "StopLoss"
   - From Entry: "Entry"

3. Target 1 Order
   - Type: Limit
   - Action: Sell
   - Quantity: 5 (50% of 10)
   - Limit Price: Entry + 1R
   - Signal Name: "Target1"
   - From Entry: "Entry"

4. Target 2 Order
   - Type: Limit
   - Action: Sell
   - Quantity: 3 (30% of 10)
   - Limit Price: Entry + 2R
   - Signal Name: "Target2"
   - From Entry: "Entry"

5. Target 3 Order
   - Type: Limit
   - Action: Sell
   - Quantity: 2 (20% of 10)
   - Limit Price: Entry + 3R
   - Signal Name: "Target3"
   - From Entry: "Entry"
```

## State Transitions

```
Panel State Machine:

[Idle] → User clicks BUY/SELL → [Validating]
  ↓
[Validating] → Inputs valid? → [Calculating]
  ↓
[Calculating] → All calculations done → [Placing Orders]
  ↓
[Placing Orders] → Orders submitted → [Trade Active]
  ↓
[Trade Active] → Monitoring breakeven & targets → [Trade Active]
  ↓
  ├─→ BE triggered → Stop moved → [Trade Active]
  ├─→ Target hit → Partial close → [Trade Active]
  └─→ All targets/stop hit → [Trade Complete] → [Idle]
```

This structure provides a complete overview of how the Trade Manager Panel is organized and operates.
