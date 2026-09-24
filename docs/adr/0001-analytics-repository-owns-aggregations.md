# ADR 0001: IAnalyticsRepository owns all aggregation reads

**Status:** Accepted  
**Date:** 2026-09-24

## Context

Two repositories both performed "how many products are below minimum stock?":
- `IProductRepository.NumberOfProductsWithLowStock()` — original location, used by the dashboard.
- `IAnalyticsRepository.GetBelowMinimumStockCountAsync()` — added when building the analytics feature.

Keeping both forced callers (dashboard handler) to know which repository to use for the same aggregated fact, and meant the same query lived in two places.

## Decision

`IAnalyticsRepository` is the single owner of all aggregation queries — counts, sums, breakdowns, and time-series that span multiple entities or compute derived values from raw data.

`IProductRepository` (and other entity repositories) own only CRUD operations on their aggregate root: add, get by id, get paged, count, existence checks. Derived aggregations belong to `IAnalyticsRepository`.

`IProductRepository.NumberOfProductsWithLowStock()` was deleted. `GetDashboardRequestHandler` now injects `IAnalyticsRepository` and calls `GetBelowMinimumStockCountAsync()`.

`IProductRepository.Top5ProductsWithLowStock()` is kept because it returns full `Product` entities with navigation properties for display purposes, not a scalar aggregation.

## Consequences

- One place to find and optimise aggregation queries.
- Dashboard handler grows one dependency (`IAnalyticsRepository`), but removes duplicated logic.
- Future aggregation needs go into `IAnalyticsRepository` by default.
