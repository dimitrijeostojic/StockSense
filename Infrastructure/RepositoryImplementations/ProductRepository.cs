using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public async Task<int> CountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.Where(p => p.TenantPublicId == tenantPublicId).CountAsync(cancellationToken);
    }

    public void Delete(Product product)
    {
        _dbContext.Products.Remove(product);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default)
    {

        var query = _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.TenantPublicId == tenantPublicId)
            .AsQueryable();

        //search
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || (p.Description != null && p.Description.Contains(searchTerm)));
        }

        //sort
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "price" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
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

    public async Task<List<Product>> GetByIdsAsync(List<int> productIds, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.StockEntries)
            .Where(p => productIds.Contains(p.Id) && p.TenantPublicId == tenantPublicId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.StockEntries)
            .FirstOrDefaultAsync(p => p.PublicId == publicId && p.TenantPublicId == tenantPublicId, cancellationToken);
    }

    public async Task<List<Product>> GetByPublicIdsAsync(IEnumerable<Guid> publicIds, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(p => publicIds.Contains(p.PublicId) && p.TenantPublicId == tenantPublicId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> NumberOfProductsWithLowStock(Guid tenantPublicId, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .Include(p => p.StockEntries)
            .Where(p => p.TenantPublicId == tenantPublicId && p.StockEntries.Sum(se => se.StockEntryType == Domain.Enums.StockEntryType.In ? +se.Quantity : -se.Quantity) < p.MinimumStockQuantity)
            .CountAsync(cancellationToken);
    }
    public async Task<ICollection<Product>> Top5ProductsWithLowStock(Guid tenantPublicId, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .Include(p => p.StockEntries)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.TenantPublicId == tenantPublicId)
            .Select(p => new
            {
                Product = p,
                CurrentStock = p.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity)
            })
           .Where(x => x.CurrentStock < x.Product.MinimumStockQuantity)
            .OrderBy(x => x.CurrentStock)
            .Take(5)
            .Select(x => x.Product)
            .ToListAsync(cancellationToken);
    }
}
