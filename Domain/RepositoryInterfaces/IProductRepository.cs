using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Delete(Product product);
}
