using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByPublicIdsAsync(IEnumerable<Guid> publicIds, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);
    void Delete(Product product);
    Task<List<Product>> GetByIdsAsync(List<int> productIds, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<int> NumberOfProductsWithLowStock(Guid tenantPublicId, CancellationToken cancellationToken);
    Task<ICollection<Product>> Top5ProductsWithLowStock(Guid tenantPublicId, CancellationToken cancellationToken);
}
