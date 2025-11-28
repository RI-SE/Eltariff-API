# Developer Guide

## Objects and how to use them

### Tariff
The main object that describes a single tariff with all its components.

### DateInterval (validPeriod)
Describes a typically longer time period (summer/winter) between two dates. Used for the property `validPeriod` in the `Tariff` object and price components.
The valid period of the `Tariff` object is most often a full year and the price components can differ and be valid only during a part of the year.

### RecurringPeriod
The `RecurringPeriods` can be added if a price component (`FixedPriceComponent`, `EnergyPriceComponent`, `PowerPriceComponent`)
is not active during all of the specified `ValidPeriod`. If no `RecurringPeriods` are added or the property is missing in the
response, the price component is active during all times within the `ValidPeriod`.

### CostFunction
The `costFunction` property of the different price objects (`FixedPrice`, `EnergyPrice`, `PowerPrice`) is a pseudo code
string that explains how to combine the `components` of each price object. To get the full cost of each price object,
the price components in the `components` array are combined as described in the `costFunction` property.

#### Methods
|Method|Description|
|-|-|
|`sum(expression(c))`|The sum of the calculated expression for each component (`c`).|
|`price(c)`|The price of a component.|
|`energy(c)`|The energy consumption/production in `unit` (e.g. kWh) for a component.|
|`peak(c)`|The power peak value for a power price component as calculated by `peakIdentificationSettings`, `activePeriods` in `recurringPeriods` and `validPeriod`. For example, the `peak(c)` value may be the average power of the three highest peak hours within a month during winter.|
|`energy(p)`|The average energy consumption/production for a period (`p`), where the period is a distinct time intervall with start and end. For example one hour or 15 minutes.|
|`power(p)`|The average power value for a period (`p`), where the period is a distinct time intervall with start and end. For example one hour or 15 minutes.|
|`price(p)`|The price power value for a period (`p`), where the period is a distinct time intervall with start and end. For example one hour or 15 minutes.|

#### Examples
|Example|Description|
|-|-|
|`sum(price(c))`|The sum of the price of each component within this price segment. Often used for the `FixedPrice` object where there are only fixed prices for a year or month where the properties `validPeriod`, `price` and `pricedPeriod` defines the price for a point int time.|
|`sum(energy(c)*price(c))`|The sum of the calculated cost for each component where the cost is the energy consumption/production multiplied with the price for each component.|
|`sum(peak(c)*price(c))`|The sum of the calculated cost for each component where the cost is the power peak multiplied with the price for each component.|

### PeakFunction
The `peakFunction` property of the `PeakIdentificationSettings` object is a pseudo code
string that explains how to calculate the power peak for a component in the `PowerPrice` object.

#### Methods
|Method|Description|
|-|-|
|`peak(r)`|The power value of the power peak for the `recurringPeriod` with `reference` name (`r`).|
|`max(peak(a),peak(b))`|Get the highest value of the result of two different peak functions.|

#### Examples
|Example|Description|
|-|-|
|`peak(main)`|Gets the peak of the `recurringPeriod` referenced as `main`. This is used when there is only one single `recurringPeriod` entry.|
|`max(peak(high),peak(low)/2)`|Selects the power peak from the `low` time period if the value is more than double the value of that of the peak from the `high` time period.|
