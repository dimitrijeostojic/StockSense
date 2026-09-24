using Domain.Enums;

namespace Domain.RepositoryInterfaces;

public interface IAnalyticsRepository
{
    Task<(IEnumerable<(Guid ProductPublicId, string ProductName, int CurrentStock, int MinimumStock)> Items, int TotalCount)> GetCurrentStockPerProductAsync(Guid tenantPublicId, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default);

    Task<int> GetBelowMinimumStockCountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);

    Task<IEnumerable<(DateTime Date, int InQuantity, int OutQuantity)>> GetStockMovementAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<(int TotalCount, decimal TotalValue)> GetOrderVolumeAndValueAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(OrderStatus Status, int Count)>> GetOrderStatusBreakdownAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid SupplierPublicId, string SupplierName, int OrderCount, decimal TotalValue)>> GetTopSuppliersAsync(Guid tenantPublicId, DateTime from, DateTime to, int topN, CancellationToken cancellationToken = default);
}
