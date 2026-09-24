using Application.AnalyticsManagement.Common;

namespace Application.AnalyticsManagement.GetBusinessAnalytics;

public record GetBusinessAnalyticsResponse(
    InventoryMetricsDto InventoryMetrics,
    OrderMetricsDto OrderMetrics);

public record InventoryMetricsDto(
    IReadOnlyList<StockLevelDto> CurrentStockPerProduct,
    int BelowMinimumCount,
    IReadOnlyList<StockMovementPointDto> StockMovement);

public record OrderMetricsDto(
    int TotalCount,
    decimal TotalValue,
    IReadOnlyList<NamedCountDto> StatusBreakdown,
    IReadOnlyList<TopSupplierDto> TopSuppliers);

public record StockLevelDto(Guid ProductPublicId, string ProductName, int CurrentStock, int MinimumStock);
public record StockMovementPointDto(DateTime Date, int InQuantity, int OutQuantity);
public record TopSupplierDto(Guid SupplierPublicId, string SupplierName, int OrderCount, decimal TotalValue);
