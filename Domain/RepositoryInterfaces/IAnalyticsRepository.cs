namespace Domain.RepositoryInterfaces;

public interface IAnalyticsRepository
{
    Task<int> GetBelowMinimumStockCountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);

    Task<IEnumerable<(DateTime Date, int InQuantity, int OutQuantity)>> GetStockMovementAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalOrderValueAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid SupplierPublicId, string SupplierName, int OrderCount, decimal TotalValue)>> GetTopSuppliersAsync(Guid tenantPublicId, DateTime from, DateTime to, int topN, CancellationToken cancellationToken = default);
}
