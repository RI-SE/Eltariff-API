# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.4.0] - 2026-10-22

### Added
- `peakSelectionPeriod` in `PeakIdentificationSettings`: the period within which a single peak is identified, e.g. `P1D` for one peak per day. It is required and must not be longer than `peakResetPeriod`.
- Catalogue API: an endpoint for getting the entire catalogue and an endpoint for version info for the catalogue.
- Documentation of `PeakIdentificationSettings` in the developer guide, with examples.

### Changed
- `fixedPrice`, `energyPrice` and `powerPrice` in `Tariff` are now optional, since a tariff may not use all price components.
- `priceIncVat` in `Price` and `PriceListEntry` is now optional, since tariffs may only provide `priceExVat`.
- **Breaking:** `peakIdentificationPeriod` in `PeakIdentificationSettings` is renamed to `peakResetPeriod`: the period over which peaks are collected and billed, after which the peaks are reset.
- `billingPeriod` in `Tariff` is now optional and informational only. It does not affect how power peaks are calculated.
- Improved descriptions of the properties in `PeakIdentificationSettings`.
- Updated the Swedish onboarding document and the example controller data to the new peak settings.

### Migration from 0.3.x
- Rename `peakIdentificationPeriod` to `peakResetPeriod`.
- Add `peakSelectionPeriod` to every `peakIdentificationSettings`. The value must not be longer than `peakResetPeriod`, e.g. `P1D` for at most one peak per day.
- `billingPeriod` can be removed from tariffs where it is not fixed for all customers.

Changes in releases before 0.4.0 are not listed here, see the [tags](https://github.com/RI-SE/Eltariff-API/tags) for earlier versions.

[Unreleased]: https://github.com/RI-SE/Eltariff-API/compare/v0.4.0...HEAD
[0.4.0]: https://github.com/RI-SE/Eltariff-API/compare/v0.3.2...v0.4.0
