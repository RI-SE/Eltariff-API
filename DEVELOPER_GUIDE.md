# Developer Guide

## Objects and how to use them

### Tariff
The main object that describes a single tariff with all its components.

### RecurringPeriod
The `RecurringPeriods` can be added if a price component (`FixedPriceComponent`, `EnergyPriceComponent`, `PowerPriceComponent`)
is not active during all of the specified `ValidPeriod`. If no `RecurringPeriods` are added or the property is missing in the
response, the price component is active during all times within the `ValidPeriod`.
