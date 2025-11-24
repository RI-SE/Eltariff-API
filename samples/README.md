# Tariff Response Samples

This directory contains sample JSON responses demonstrating various electricity tariff structures supported by the Eltariff API.

## Tariff Structure Examples

### Basic Tariff Components

**tariffs-response-jamtkraft.json**
- Simple fuse-based tariff (säkringstariff)
- Fixed annual subscription fee based on fuse size (16A)
- Fixed energy price per kWh with energy tax
- Zero-priced power component (currently)

**tariffs-response_HEM.json**
- Multiple fuse size variants (16A, 20A, 25A)
- Simple flat-rate energy transfer pricing
- No power/demand charges

### Time-of-Use (TOU) Tariffs

**tariffs-response_DE.json** (Dala Energi)
- Seasonal pricing (winter/summer periods)
- Time-differentiated with weekday/weekend distinctions
- High-load periods (weekdays 07:00-20:00) vs low-load periods
- Calendar pattern references (holidays excluded from high-load pricing)
- Peak demand charges based on 3-peak monthly average

**tariffs-response.json**
Multiple sophisticated tariff variants:
- **Fixed price with daily peaks**: Same price year-round with 3-peak average calculation
- **Spot price following**: Dynamic energy component that tracks spot market prices
- **Day/night differential**: Higher prices during 07:00-20:00, lower at night
- **Seasonal variations**: Different pricing during winter (Oct-Apr) vs summer
- **Complex peak functions**: Including `max(peak(high),peak(low)/2)` logic

**tariffs-response_goteborg-energi.json**
- Simple monthly peak demand structure
- Fixed subscription and energy transfer fees
- Single peak per month billing

### Dynamic Pricing

**tariffs-response.json** (Dynamiskt timpris)
- Hourly dynamic pricing during winter season
- Individual price levels updated daily for each hour
- Zero pricing during summer months
- Fuse-size based fixed fee (16A)

**prices-response.json**
- Hourly spot price data for specific component
- Demonstrates how dynamic pricing components receive actual price values
- 24-hour price schedule with varying rates

## Common Elements Across Tariffs

All tariff structures include:
- **Fixed prices**: Monthly or annual subscription fees
- **Energy prices**: Per-kWh charges with energy tax component
- **Power prices**: Demand charges based on peak consumption (where applicable)
- **Valid periods**: Date ranges for price applicability
- **Billing periods**: Typically monthly (P1M)
- **Time zones**: All samples use Europe/Stockholm

## Tariff Complexity Features

- Calendar pattern support (weekdays/weekends/holidays)
- Multiple recurring periods within a day
- Peak identification with configurable duration and averaging
- Spot price multipliers
- Cost functions (sum, max, custom formulas)
- Seasonal validity periods