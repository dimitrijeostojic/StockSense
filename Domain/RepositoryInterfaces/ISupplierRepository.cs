using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface ISupplierRepository
{
    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Supplier> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Delete(Supplier supplier);
}
