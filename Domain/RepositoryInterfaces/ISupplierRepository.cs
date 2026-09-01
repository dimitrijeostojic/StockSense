using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface ISupplierRepository
{
    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Supplier> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, string? filterOn = null, string? filterQuery = null, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);
    Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default);
}
