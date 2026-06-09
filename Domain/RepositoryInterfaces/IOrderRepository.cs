using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, Guid tenantPublicId, CancellationToken cancellationToken = default);
    void Delete(Order order);
    Task<int> GetNumberOfActiveOrders(Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<ICollection<Order>> GetLatestOrders(Guid tenantPublicId, CancellationToken cancellationToken = default);
}
