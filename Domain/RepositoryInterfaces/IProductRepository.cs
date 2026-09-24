using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByPublicIdsAsync(IEnumerable<Guid> publicIds, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, string? filterOn = null, string? filterQuery = null, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByIdsAsync(List<int> productIds, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<ICollection<(Product Product, int CurrentStock)>> Top5ProductsWithLowStock(Guid tenantPublicId, CancellationToken cancellationToken);
    Task<bool> ExistsBySkuAsync(Guid tenantPublicId, string sku, CancellationToken cancellationToken);
    Task<HashSet<string>> GetAllSkusAsync(Guid tenantPublicId, CancellationToken cancellationToken);
    Task<bool> AnyByCategoryIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> AnyBySupplierIdAsync(int id, CancellationToken cancellationToken);
}
