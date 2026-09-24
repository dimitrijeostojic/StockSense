namespace Application.AnalyticsManagement.GetBusinessAnalytics;

public record GetBusinessAnalyticsResponse(
    InventoryMetricsDto InventoryMetrics,
    OrderMetricsDto OrderMetrics);

public record InventoryMetricsDto(
    IReadOnlyList<StockMovementPointDto> StockMovement);

public record OrderMetricsDto(
    decimal TotalValue,
    IReadOnlyList<TopSupplierDto> TopSuppliers);

public record StockMovementPointDto(DateTime Date, int InQuantity, int OutQuantity);
public record TopSupplierDto(Guid SupplierPublicId, string SupplierName, int OrderCount, decimal TotalValue);
