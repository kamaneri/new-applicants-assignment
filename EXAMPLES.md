# Trade Manager Panel - Examples and Use Cases

## Example 1: Basic Long Trade Setup

### Scenario
- **Instrument**: ES (E-mini S&P 500) Futures
- **Account Balance**: $25,000
- **Account Risk**: 1% = $250
- **Current Price**: 4,500.00
- **Current Candle Range**: High 4,502.00, Low 4,498.00 = 4 points = 16 ticks
- **Tick Value**: $12.50
- **Settings**: 
  - Stop Loss Tick Buffer: 2 ticks
  - Number of Targets: 3
  - Distribution: 50% / 30% / 20%
  - Auto BE: Enabled, Risk Based (1R), Buffer: 2 ticks

### User Action
Click **BUY** button

### Panel Calculations

1. **Stop Loss Calculation**
   ```
   Candle Range: 16 ticks
   Buffer: 2 ticks
   Stop Loss Distance: 16 + 2 = 18 ticks
   Stop Loss Distance in points: 18 × 0.25 = 4.5 points
   Stop Loss Price: 4,500.00 - 4.5 = 4,495.50
   ```

2. **Target Calculations**
   ```
   Target 1 (1R): 4,500.00 + 4.5 = 4,504.50
   Target 2 (2R): 4,500.00 + 9.0 = 4,509.00
   Target 3 (3R): 4,500.00 + 13.5 = 4,513.50
   Target 4 (4R): 4,500.00 + 18.0 = 4,518.00 (visual only)
   ```

3. **Position Sizing**
   ```
   Risk Amount: $250
   Risk Per Contract: 18 ticks × $12.50 = $225
   Total Contracts: floor($250 / $225) = 1 contract
   ```

4. **Contract Distribution**
   ```
   Target 1 Contracts: 1 × 50% = 1 (minimum 1)
   Target 2 Contracts: 0
   Target 3 Contracts: 0
   
   Note: With only 1 contract, all goes to Target 1
   ```

5. **Breakeven Setup**
   ```
   Mode: Risk Based (1R)
   Trigger Price: 4,500.00 + 4.5 = 4,504.50
   New Stop When Triggered: 4,500.00 + (2 ticks × 0.25) = 4,500.50
   ```

### Orders Placed
1. **Entry**: Buy 1 @ Market (fills at 4,500.00)
2. **Stop Loss**: Sell Stop 1 @ 4,495.50
3. **Target 1**: Sell Limit 1 @ 4,504.50

### Trade Progression

**Scenario A: Winner**
- Price rises to 4,504.50
- Target 1 filled → Exit 1 contract at 4,504.50
- Stop moved to 4,500.50 (breakeven + 2 ticks)
- **Result**: +$56.25 profit (4.5 points × $12.50), risk-free after BE

**Scenario B: Stop Hit Before Target**
- Price drops to 4,495.50
- Stop Loss filled → Exit 1 contract at 4,495.50
- **Result**: -$225 loss (18 ticks × $12.50), exactly 1% of account

---

## Example 2: Short Trade with Multiple Contracts

### Scenario
- **Instrument**: NQ (E-mini NASDAQ 100) Futures
- **Account Balance**: $50,000
- **Account Risk**: 2% = $1,000
- **Current Price**: 15,000.00
- **Current Candle Range**: High 15,004.00, Low 14,996.00 = 8 points = 32 ticks
- **Tick Value**: $5.00
- **Settings**:
  - Stop Loss Tick Buffer: 4 ticks
  - Number of Targets: 3
  - Distribution: 50% / 30% / 20%
  - Auto BE: Enabled, Fixed Ticks (20), Buffer: 4 ticks

### User Action
Click **SELL** button

### Panel Calculations

1. **Stop Loss Calculation**
   ```
   Candle Range: 32 ticks
   Buffer: 4 ticks
   Stop Loss Distance: 32 + 4 = 36 ticks
   Stop Loss Distance in points: 36 × 0.25 = 9 points
   Stop Loss Price: 15,000.00 + 9 = 15,009.00
   ```

2. **Target Calculations**
   ```
   Target 1 (1R): 15,000.00 - 9 = 14,991.00
   Target 2 (2R): 15,000.00 - 18 = 14,982.00
   Target 3 (3R): 15,000.00 - 27 = 14,973.00
   Target 4 (4R): 15,000.00 - 36 = 14,964.00 (visual only)
   ```

3. **Position Sizing**
   ```
   Risk Amount: $1,000
   Risk Per Contract: 36 ticks × $5.00 = $180
   Total Contracts: floor($1,000 / $180) = 5 contracts
   ```

4. **Contract Distribution**
   ```
   Target 1 Contracts: 5 × 50% = 2.5 → 3 (rounded)
   Target 2 Contracts: 5 × 30% = 1.5 → 2 (rounded)
   Target 3 Contracts: 5 - 3 - 2 = 0
   
   Adjusted:
   Target 1: 3 contracts (60%)
   Target 2: 2 contracts (40%)
   Target 3: 0 contracts
   ```

5. **Breakeven Setup**
   ```
   Mode: Fixed Ticks
   Trigger Price: 15,000.00 - (20 ticks × 0.25) = 14,995.00
   New Stop When Triggered: 15,000.00 - (4 ticks × 0.25) = 14,999.00
   ```

### Orders Placed
1. **Entry**: Sell Short 5 @ Market (fills at 15,000.00)
2. **Stop Loss**: Buy Stop 5 @ 15,009.00
3. **Target 1**: Buy Limit 3 @ 14,991.00
4. **Target 2**: Buy Limit 2 @ 14,982.00

### Trade Progression

**Optimal Scenario**
1. Entry at 15,000.00 with 5 contracts
2. Price drops to 14,995.00 (20 ticks)
   - Breakeven triggers
   - Stop moves to 14,999.00 (entry - 4 ticks)
3. Price drops to 14,991.00
   - Target 1 fills: 3 contracts close
   - Profit: 3 × 36 ticks × $5 = $540
   - Stop remains at 14,999.00 for remaining 2 contracts
4. Price drops to 14,982.00
   - Target 2 fills: 2 contracts close
   - Profit: 2 × 72 ticks × $5 = $720
5. **Total Profit**: $540 + $720 = $1,260
6. **R:R Result**: Risked $900 (5 × $180), Made $1,260 = 1.4R

---

## Example 3: Conservative Single Target Setup

### Scenario
- **Instrument**: CL (Crude Oil) Futures
- **Account Balance**: $15,000
- **Account Risk**: 0.5% = $75
- **Current Price**: 80.00
- **Current Candle Range**: High 80.15, Low 79.90 = $0.25 = 25 ticks
- **Tick Value**: $10.00
- **Settings**:
  - Stop Loss Tick Buffer: 3 ticks
  - Number of Targets: 1
  - Distribution: 100%
  - Auto BE: Disabled

### User Action
Click **BUY** button

### Panel Calculations

1. **Stop Loss Calculation**
   ```
   Candle Range: 25 ticks
   Buffer: 3 ticks
   Stop Loss Distance: 25 + 3 = 28 ticks
   Stop Loss Distance in dollars: 28 × $0.01 = $0.28
   Stop Loss Price: 80.00 - 0.28 = 79.72
   ```

2. **Target Calculations**
   ```
   Target 1 (1R): 80.00 + 0.28 = 80.28
   Target 2 (2R): 80.00 + 0.56 = 80.56 (not used)
   Target 3 (3R): 80.00 + 0.84 = 80.84 (not used)
   ```

3. **Position Sizing**
   ```
   Risk Amount: $75
   Risk Per Contract: 28 ticks × $10 = $280
   Total Contracts: floor($75 / $280) = 0 → 1 (minimum)
   
   Note: Insufficient balance for 1% risk, using minimum 1 contract
   Actual Risk: $280 (1.87% of account)
   ```

4. **Contract Distribution**
   ```
   Target 1: 1 contract (100%)
   ```

### Orders Placed
1. **Entry**: Buy 1 @ Market (fills at 80.00)
2. **Stop Loss**: Sell Stop 1 @ 79.72
3. **Target 1**: Sell Limit 1 @ 80.28

### Trade Progression

**Scenario A: Target Hit**
- Price rises to 80.28
- Target 1 filled
- **Profit**: $280 (28 ticks × $10)
- **Actual Gain**: 1.87% of account

**Scenario B: Stop Hit**
- Price drops to 79.72
- Stop Loss filled
- **Loss**: $280
- **Actual Loss**: 1.87% of account

**Note**: This demonstrates the importance of adequate account size for desired risk percentage.

---

## Example 4: Breakeven Saves the Day

### Scenario
- **Instrument**: ES Futures
- **Account**: $30,000
- **Risk**: 1% = $300
- **Entry**: 4,600.00 (LONG)
- **Candle Range**: 10 ticks
- **Buffer**: 2 ticks
- **Stop Distance**: 12 ticks = 3 points
- **Contracts**: 2
- **Targets**: 3 (50% / 30% / 20%)
- **Auto BE**: Risk Based (1R), Buffer 2 ticks

### Setup
```
Entry: 4,600.00 (2 contracts)
Stop Loss: 4,597.00 (12 ticks below)
Target 1: 4,603.00 (12 ticks above) - 1 contract
Target 2: 4,606.00 (24 ticks above) - 0.6 → 1 contract
Target 3: 4,609.00 (36 ticks above) - 0.4 → 0 contracts
BE Trigger: 4,603.00 (1R = 12 ticks)
BE Stop: 4,600.50 (entry + 2 ticks)
```

### Trade Progression Timeline

**Time 0:00** - Entry executed at 4,600.00
- Position: Long 2 contracts
- Risk: 2 × 12 ticks × $12.50 = $300

**Time 0:15** - Price rises to 4,603.00
- Target 1 hit: 1 contract closes
- Profit on Target 1: 12 ticks × $12.50 = $150
- Breakeven triggers: Stop moves from 4,597.00 to 4,600.50
- Remaining: 1 contract

**Time 0:30** - Price pulls back to 4,601.00
- No fills
- Remaining: 1 contract at breakeven + 2 ticks protection

**Time 0:45** - Price drops further to 4,600.50
- Breakeven stop hit: 1 contract closes at 4,600.50
- Result on 2nd contract: +2 ticks × $12.50 = +$25
- **Total Trade Result**: $150 + $25 = $175 profit

**Analysis**:
- Without breakeven: Would have lost $300 (both contracts stopped at 4,597.00)
- With breakeven: Made $175 profit
- **Difference**: $475 saved by auto breakeven feature!

---

## Example 5: Scaling Out Strategy

### Scenario
- **Instrument**: 6E (Euro FX) Futures
- **Account**: $40,000
- **Risk**: 1% = $400
- **Current Price**: 1.1000
- **Candle Range**: 0.0008 = 8 pips
- **Tick Value**: $12.50
- **Settings**:
  - Buffer: 1 pip
  - Targets: 3
  - Distribution: 40% / 35% / 25%
  - Auto BE: Risk Based, Buffer 1 pip

### Calculations
```
Stop Distance: 8 + 1 = 9 pips = 0.0009
Stop Price: 1.1000 - 0.0009 = 1.0991
Target 1 (1R): 1.1000 + 0.0009 = 1.1009
Target 2 (2R): 1.1000 + 0.0018 = 1.1018
Target 3 (3R): 1.1000 + 0.0027 = 1.1027

Risk Per Contract: 9 pips × $12.50 = $112.50
Contracts: $400 / $112.50 = 3.55 → 3 contracts

Distribution:
Target 1: 3 × 40% = 1.2 → 1 contract
Target 2: 3 × 35% = 1.05 → 1 contract  
Target 3: 3 - 1 - 1 = 1 contract
```

### Trade Progression

**Phase 1**: Price reaches 1.1009 (Target 1)
- 1 contract closes at Target 1
- Profit: 9 pips × $12.50 = $112.50
- Stop moves to 1.1001 (entry + 1 pip)
- Remaining: 2 contracts

**Phase 2**: Price reaches 1.1018 (Target 2)
- 1 contract closes at Target 2
- Profit: 18 pips × $12.50 = $225
- Stop still at 1.1001
- Remaining: 1 contract

**Phase 3**: Price reaches 1.1027 (Target 3)
- 1 contract closes at Target 3
- Profit: 27 pips × $12.50 = $337.50

**Total Profit**: $112.50 + $225 + $337.50 = $675
**R:R**: Risked $337.50 (3 × $112.50), Made $675 = 2R

---

## Example 6: Small Account Management

### Scenario
- **Account**: $5,000
- **Risk**: 1% = $50
- **Instrument**: Micro ES (MES)
- **Tick Value**: $1.25
- **Current Price**: 4,500.00
- **Candle Range**: 8 ticks
- **Buffer**: 2 ticks

### Issue
```
Stop Distance: 8 + 2 = 10 ticks
Risk Per Contract: 10 × $1.25 = $12.50
Contracts for 1% risk: $50 / $12.50 = 4 contracts
```

### Solution: Use Micro Contracts
With micro contracts, this small account can:
- Trade 4 contracts
- Maintain 1% risk
- Use 2-3 targets effectively

### Recommended Settings for Small Accounts
```
Number of Targets: 2
Distribution: 50% / 50%
Target 1: 2 contracts at 1R (quick profit)
Target 2: 2 contracts at 2R (let winners run)
Auto BE: Enabled, Fixed Ticks (8-10)
```

---

## Common Scenarios and Responses

### Scenario: Whipsaw Market
**Setup**: Choppy, ranging market
**Response**: 
- Increase tick buffer (3-5 ticks)
- Use Fixed Tick BE mode with larger value (15-20 ticks)
- Consider 1 target only at 1R

### Scenario: Trending Market
**Setup**: Strong directional movement
**Response**:
- Use 3 targets
- Smaller percentages on early targets (30%/30%/40%)
- Risk Based BE mode
- Let final target run

### Scenario: High Volatility
**Setup**: Large candle ranges
**Response**:
- Reduce tick buffer (1-2 ticks) - range already large
- Tighten target distribution
- Lower risk percentage (0.5%)

### Scenario: News Event
**Setup**: Major economic release expected
**Response**:
- Avoid trading just before/during news
- If in trade, tighten stops manually
- Consider exiting partially before news

---

## Performance Metrics Examples

### Conservative Trader Profile
```
Account Risk: 0.5%
Targets: 2 (60% / 40%)
BE Mode: Fixed Ticks (15)
Win Rate: 65%
Average R: 1.3R

Monthly Return: ~4-6% (compounded)
Max Drawdown: ~5%
```

### Aggressive Trader Profile
```
Account Risk: 2%
Targets: 3 (40% / 30% / 30%)
BE Mode: Risk Based (1R)
Win Rate: 55%
Average R: 1.8R

Monthly Return: ~12-18% (compounded)
Max Drawdown: ~12-15%
```

### Recommended Starting Profile
```
Account Risk: 1%
Targets: 3 (50% / 30% / 20%)
BE Mode: Risk Based (1R), Buffer 2
Win Rate Target: 55-60%
Average R Target: 1.5R

Monthly Return Goal: ~6-10%
Max Acceptable Drawdown: ~8%
```

---

These examples demonstrate the flexibility and power of the Trade Manager Panel in various market conditions and account sizes.
