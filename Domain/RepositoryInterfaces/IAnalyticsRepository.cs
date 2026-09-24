using Domain.Enums;

namespace Domain.RepositoryInterfaces;

public interface IAnalyticsRepository
{
    Task<List<string>> GetUserIdsByTenantAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);

    Task<IEnumerable<(string EntityName, int Count)>> GetActivityByEntityTypeAsync(List<string> userIds, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(string Action, int Count)>> GetActivityByActionTypeAsync(List<string> userIds, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(string UserEmail, int Count)>> GetTopActiveUsersAsync(List<string> userIds, DateTime from, DateTime to, int topN, CancellationToken cancellationToken = default);

    Task<IEnumerable<(string Period, int Count)>> GetRegistrationTrendAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid ProductPublicId, string ProductName, int CurrentStock, int MinimumStock)>> GetCurrentStockPerProductAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);

    Task<int> GetBelowMinimumStockCountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);

    Task<IEnumerable<(DateTime Date, int InQuantity, int OutQuantity)>> GetStockMovementAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<(int TotalCount, decimal TotalValue)> GetOrderVolumeAndValueAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(OrderStatus Status, int Count)>> GetOrderStatusBreakdownAsync(Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid SupplierPublicId, string SupplierName, int OrderCount, decimal TotalValue)>> GetTopSuppliersAsync(Guid tenantPublicId, DateTime from, DateTime to, int topN, CancellationToken cancellationToken = default);
}
