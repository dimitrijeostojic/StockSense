using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class SupplierRepository(ApplicationDbContext dbContext) : ISupplierRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        await _dbContext.Suppliers.AddAsync(supplier, cancellationToken);
    }

    public Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _dbContext.Suppliers.Remove(supplier);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Suppliers.Where(o => o.TenantPublicId == tenantPublicId).AsQueryable();

        //search
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        //sort
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "email" => isAscending ? query.OrderBy(p => p.ContactEmail) : query.OrderByDescending(p => p.ContactEmail),
                "createdat" => isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                _ => query
            };
        }
        var totalCount = await query.CountAsync(cancellationToken);

        //pagination
        var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Supplier?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.PublicId == publicId && s.TenantPublicId == tenantPublicId, cancellationToken);
    }
}
