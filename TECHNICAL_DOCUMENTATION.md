# Trade Manager Panel - Technical Documentation

## Architecture Overview

The Trade Manager Panel is built as a NinjaTrader 8 Add-On using C# and WPF (Windows Presentation Foundation) for the user interface.

### Key Components

1. **UI Layer** (WPF Controls)
   - Main panel grid layout
   - Input controls (TextBox, CheckBox, ComboBox)
   - Execution buttons (Buy/Sell)
   - Status displays

2. **Calculation Engine**
   - Stop loss calculator
   - R:R profit target calculator
   - Position sizing calculator
   - Contract distribution logic

3. **Order Management**
   - Market order execution
   - Stop loss order placement
   - Limit target order placement
   - Order modification for breakeven

4. **Visual Display**
   - Chart line drawing
   - Real-time price projection
   - Visual feedback system

5. **Monitoring System**
   - Breakeven trigger detection
   - Position tracking
   - Status updates

## Code Structure

### Main Classes and Methods

```csharp
public class TradeManagerPanel : AddOnBase
{
    // Panel Creation
    - AddPanelToChart()
    - CreateMainPanel()
    - AddTitleLabel()
    - AddSectionLabel()
    - AddLabeledTextBox()
    - AddTargetCountSelector()
    - AddBreakevenControls()
    - AddExecutionButtons()
    
    // Trade Execution
    - ExecuteTrade()
    - ReadUserInputs()
    - GetMarketData()
    - CalculateStopLoss()
    - CalculateProfitTargets()
    - CalculatePositionSize()
    - DistributeContracts()
    - PlaceOrders()
    
    // Visual Display
    - DisplayVisualProjections()
    - DrawHorizontalLine()
    - ClearVisualProjections()
    
    // Breakeven Logic
    - MonitorForBreakeven()
    - MoveStopToBreakeven()
    
    // UI Updates
    - UpdateStatus()
    - UpdateInfoLabel()
}
```

## Calculation Algorithms

### 1. Stop Loss Calculation

**Formula:**
```
lastCandleSize = lastCandleHigh - lastCandleLow
bufferAmount = stopLossBufferTicks × tickSize
stopDistance = lastCandleSize + bufferAmount

For Long:  stopLossPrice = entryPrice - stopDistance
For Short: stopLossPrice = entryPrice + stopDistance
```

**Example:**
- Last candle: High = 4500, Low = 4490 (10 points)
- Buffer: 2 ticks × 0.25 = 0.5 points
- Entry: 4502
- Stop Distance: 10 + 0.5 = 10.5 points
- Stop Loss (Long): 4502 - 10.5 = 4491.5

### 2. Profit Target Calculation (R:R)

**Formula:**
```
R = riskDistance = |entryPrice - stopLossPrice|

Target1 (1R) = entryPrice + (R × 1)
Target2 (2R) = entryPrice + (R × 2)
Target3 (3R) = entryPrice + (R × 3)

For Short positions, subtract instead of add
```

**Example:**
- Entry: 4502
- Stop Loss: 4491.5
- R = 4502 - 4491.5 = 10.5 points
- Target 1 (1R): 4502 + 10.5 = 4512.5
- Target 2 (2R): 4502 + 21.0 = 4523.0
- Target 3 (3R): 4502 + 31.5 = 4533.5

### 3. Position Sizing

**Formula:**
```
accountBalance = account.CashValue
dollarRisk = accountBalance × (riskPercentage / 100)
riskPerContract = |entryPrice - stopLossPrice| × pointValue
totalContracts = floor(dollarRisk / riskPerContract)
```

**Example:**
- Account: $25,000
- Risk: 1% = $250
- Entry: 4502
- Stop: 4491.5
- Risk per contract: 10.5 × $50 = $525
- Contracts: floor($250 / $525) = 0 contracts (insufficient capital for 1% risk)
- Adjusted to minimum: 1 contract

**Example 2:**
- Account: $50,000
- Risk: 1% = $500
- Entry: 4502
- Stop: 4497 (5 points)
- Risk per contract: 5 × $50 = $250
- Contracts: floor($500 / $250) = 2 contracts

### 4. Contract Distribution

**Algorithm:**
```
For each target i (0 to targetCount-1):
    if i == last target:
        contractsPerTarget[i] = remainingContracts
    else:
        contractsPerTarget[i] = floor(totalContracts × targetPercentages[i])
        remainingContracts -= contractsPerTarget[i]
```

**Example:**
- Total Contracts: 10
- 3 Targets with 50%, 30%, 20%
- Target 1: floor(10 × 0.50) = 5 contracts
- Target 2: floor(10 × 0.30) = 3 contracts
- Target 3: Remaining = 2 contracts (ensures all contracts used)

### 5. Breakeven Triggers

**Mode 1 - Fixed Ticks:**
```
breakevenDistance = breakevenTicks × tickSize

For Long:  trigger when currentPrice >= entryPrice + breakevenDistance
For Short: trigger when currentPrice <= entryPrice - breakevenDistance

New Stop Price:
For Long:  entryPrice + (breakevenBufferTicks × tickSize)
For Short: entryPrice - (breakevenBufferTicks × tickSize)
```

**Mode 2 - Risk-Based (1R):**
```
R = |entryPrice - stopLossPrice|

For Long:  trigger when currentPrice >= entryPrice + R
For Short: trigger when currentPrice <= entryPrice - R

New Stop Price:
For Long:  entryPrice + (breakevenBufferTicks × tickSize)
For Short: entryPrice - (breakevenBufferTicks × tickSize)
```

## UI Component Reference

### Input Fields

| Field | Type | Default | Range | Description |
|-------|------|---------|-------|-------------|
| Risk % | TextBox | 1.0 | 0.1-100 | Percentage of account to risk |
| Stop Buffer | TextBox | 2 | 0-100 | Ticks to add to stop distance |
| Target Count | ComboBox | 3 | 1-3 | Number of profit targets |
| Target 1 % | TextBox | 50 | 1-100 | Percentage for first target |
| Target 2 % | TextBox | 30 | 1-100 | Percentage for second target |
| Target 3 % | TextBox | 20 | 1-100 | Percentage for third target |
| Enable Auto BE | CheckBox | false | - | Enable auto breakeven |
| BE Mode | ComboBox | Fixed | 1-2 | Breakeven trigger mode |
| BE Ticks | TextBox | 10 | 1-1000 | Ticks for Mode 1 trigger |
| BE Buffer | TextBox | 5 | 0-100 | Buffer ticks for breakeven |

### Status Indicators

**Status Label Colors:**
- Green: Successful operation
- Yellow: Processing/In progress
- Red: Error condition
- Blue: Informational update

**Info Label Format:**
```
"Contracts: {totalContracts} | Targets: {targetCount} | R:R 1:{targetCount}"
```

## Visual Elements

### Chart Lines

| Element | Color | Width | Description |
|---------|-------|-------|-------------|
| Stop Loss | Red | 2px | Current stop loss level |
| Entry | White | 1px | Entry price level |
| Target 1 | Lime Green | 2px | First profit target (1R) |
| Target 2 | Yellow | 2px | Second profit target (2R) |
| Target 3 | Orange | 2px | Third profit target (3R) |
| Breakeven | Blue | 2px | Stop after BE trigger |

### Drawing Implementation

Uses NinjaTrader's `Draw.HorizontalLine()` API:
```csharp
Draw.HorizontalLine(
    chart,           // Chart object
    tag,            // Unique identifier
    price,          // Y-axis price level
    brush           // Line color
)
```

## Event Flow

### Trade Execution Sequence

```
1. User clicks BUY or SELL button
   ↓
2. ExecuteTrade(isLong) called
   ↓
3. ReadUserInputs() - Validates all input fields
   ↓
4. GetMarketData() - Retrieves current price and instrument
   ↓
5. CalculateStopLoss() - Based on candlestick + buffer
   ↓
6. CalculateProfitTargets() - 1R, 2R, 3R from stop distance
   ↓
7. CalculatePositionSize() - Based on account risk %
   ↓
8. DistributeContracts() - Across selected targets
   ↓
9. DisplayVisualProjections() - Draw lines on chart
   ↓
10. PlaceOrders() - Submit all orders to account
    ↓
11. PlaceStopLossOrder() - Stop market order
    ↓
12. PlaceProfitTargetOrders() - Limit orders for each target
    ↓
13. MonitorForBreakeven() - If enabled, start monitoring
    ↓
14. UpdateStatus() - Display success message
```

### Breakeven Monitoring Flow

```
1. Position opened
   ↓
2. MonitorForBreakeven() activated (if enabled)
   ↓
3. On each price update:
   ↓
4. Check trigger condition:
   - Mode 1: Price moved X ticks?
   - Mode 2: Price moved 1R?
   ↓
5. If triggered:
   ↓
6. MoveStopToBreakeven()
   ↓
7. Calculate new stop price (entry + buffer)
   ↓
8. Modify stop loss order
   ↓
9. Update visual display
   ↓
10. Update status message
```

## Error Handling

### Validation Checks

1. **Input Validation:**
   - All numeric fields must be positive numbers
   - Target percentages must sum to reasonable total
   - Risk percentage must be > 0 and < 100

2. **Market Data Checks:**
   - Current price available and valid
   - Instrument loaded and tradeable
   - Account connected and active

3. **Position Sizing Checks:**
   - At least 1 contract can be traded
   - Sufficient buying power
   - Stop loss distance is reasonable

4. **Order Placement Checks:**
   - Orders created successfully
   - Order submission accepted
   - No rejected orders

### Error Messages

```csharp
"Error: Invalid input values"
"Error: Unable to get market data"
"Error: Insufficient buying power"
"Order Error: {exception.Message}"
"Error: {general exception}"
```

## Performance Optimization

### Optimization Techniques Used

1. **Lazy Loading:** UI elements created only when panel is added
2. **Async Operations:** UI updates use Dispatcher.InvokeAsync()
3. **Efficient Drawing:** Visual elements stored in dictionary for quick access
4. **Minimal Redraws:** Only update visuals when necessary
5. **Object Pooling:** Reuse order objects where possible

### Memory Management

- Visual elements cleaned up on panel removal
- Event handlers properly unsubscribed
- Orders tracked but not cached unnecessarily
- UI controls disposed with panel

## Extension Points

### Easy Customizations

1. **Add More Targets:**
   - Increase `profitTargets` array size
   - Add 4R, 5R calculations
   - Add UI fields for additional targets

2. **Custom Stop Calculations:**
   - Modify `CalculateStopLoss()` method
   - Add ATR-based stops
   - Add swing high/low stops

3. **Additional Breakeven Modes:**
   - Add Mode 3, Mode 4, etc.
   - Trailing stop functionality
   - Dynamic stop adjustment

4. **Enhanced Visuals:**
   - Add text labels to lines
   - Show dollar amounts
   - Add risk/reward ratio display

5. **Advanced Position Management:**
   - Scale in logic
   - Pyramiding functionality
   - Partial profit taking rules

## Integration with NinjaTrader

### Required NinjaTrader APIs

```csharp
using NinjaTrader.Cbi;              // Account, Order, Position
using NinjaTrader.Gui.Chart;        // Chart controls
using NinjaTrader.Data;             // Market data
using NinjaTrader.NinjaScript.DrawingTools; // Visual drawing
```

### Account Methods Used

- `account.Get()` - Retrieve account information
- `account.CreateOrder()` - Create new order object
- `account.Submit()` - Submit orders to broker
- `account.Change()` - Modify existing orders

### Chart Methods Used

- `chartControl.GetCurrentAsk()` - Get current ask price
- `chartControl.GetCurrentBid()` - Get current bid price
- `chartControl.BarsArray` - Access price bars
- `Draw.HorizontalLine()` - Draw lines on chart

## Testing Recommendations

### Unit Testing Checklist

- [ ] Stop loss calculation accuracy
- [ ] Profit target calculation accuracy
- [ ] Position sizing calculation accuracy
- [ ] Contract distribution logic
- [ ] Percentage validation
- [ ] Input range validation

### Integration Testing Checklist

- [ ] Panel displays correctly on chart
- [ ] All UI controls functional
- [ ] Orders place successfully in simulation
- [ ] Visual lines appear correctly
- [ ] Status updates display properly
- [ ] Error handling works correctly

### User Acceptance Testing

- [ ] Test with various account sizes
- [ ] Test with different risk percentages
- [ ] Test all target count options (1, 2, 3)
- [ ] Test both breakeven modes
- [ ] Test with different instruments
- [ ] Test long and short positions
- [ ] Verify visual clarity and usability

## Maintenance Notes

### Known Considerations

1. **NinjaTrader Version:** Built for NinjaTrader 8, may need updates for NT9+
2. **API Changes:** NinjaTrader API updates may require code adjustments
3. **Broker Compatibility:** Some brokers may have specific order requirements
4. **Market Hours:** Panel assumes market is open when orders are placed

### Future Enhancements

1. Add OCO bracket order support
2. Implement trailing stop logic
3. Add trade journaling functionality
4. Create configuration profiles
5. Add sound alerts
6. Implement email/SMS notifications
7. Add performance statistics display
8. Create mobile app integration

---

**Document Version:** 1.0  
**Last Updated:** 2026-02-07  
**Author:** Trade Manager Panel Development Team
