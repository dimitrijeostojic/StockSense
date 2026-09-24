# StockSense Domain Glossary

## Core Concepts

**Tenant** — an isolated business account. All data (products, orders, stock, users) belongs to exactly one tenant. Analytics are always scoped to the requesting user's tenant.

**Tenant Admin** — a user with the Admin role. Sees and manages only their own tenant's data. Distinct from a platform-level super-admin (which does not exist in this system).

**User** — a user with the User role, belonging to a tenant.

## Analytics Domain

**Analytics** — a dedicated read-only section of the application (separate from Dashboard) providing deeper insights into business performance for any authenticated user of a tenant. Filtered by a time range (preset or custom).

**Business Analytics** — the analytics section of Dashboard covering:
- *Inventory Metrics* — stock movement over time (In vs Out).
- *Order Metrics* — total order value over time, top suppliers by order count.

**Stock Movement** — a StockEntry record with type In or Out. Current stock level for a product = Σ In quantities − Σ Out quantities.

**Order Value** — the monetary total of an order = Σ (OrderItem.UnitPrice × OrderItem.Quantity × (1 + OrderItem.VatRate)) across all OrderItems belonging to that order.

**Time Range** — a filter applied to all analytics queries. Supported as fixed presets (7d / 30d / 90d / 365d) and as a custom from/to date pair.
