using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByPublicIdsAsync(IEnumerable<Guid> publicIds, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, Guid tenantPublicId, CancellationToken cancellationToken = default);
    void Delete(Product product);
    Task<List<Product>> GetByIdsAsync(List<int> productIds, Guid tenantPublicId, CancellationToken cancellationToken);
}
