using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, string? filterOn = null, string? filterQuery = null, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);
    void Delete(Order order);
    Task<int> GetNumberOfActiveOrders(Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<ICollection<Order>> GetLatestOrders(Guid tenantPublicId, CancellationToken cancellationToken = default);
}
