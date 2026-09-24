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
        return await _dbContext.Products.Where(p => p.TenantPublicId == tenantPublicId && p.IsActive).CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(Guid tenantPublicId, string sku, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.AnyAsync(p => p.TenantPublicId == tenantPublicId && p.Sku == sku && p.IsActive, cancellationToken);
    }

    public async Task<HashSet<string>> GetAllSkusAsync(Guid tenantPublicId, CancellationToken cancellationToken)
    {
        var skus = await _dbContext.Products
            .Where(p => p.TenantPublicId == tenantPublicId && p.IsActive)
            .Select(p => p.Sku)
            .ToListAsync(cancellationToken);
        return new HashSet<string>(skus, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, string? filterOn = null, string? filterQuery = null, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default)
    {

        var query = _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.StockEntries)
            .Where(p => p.TenantPublicId == tenantPublicId && p.IsActive)
            .AsQueryable();

        //search
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                (p.Description != null
                    && p.Description.Contains(searchTerm)) ||
                (p.Sku != null
                    && p.Sku.Contains(searchTerm)));
        }

        //filter
        if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
        {
            if (filterOn.Equals("Category", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Category != null && x.Category.Name.Contains(filterQuery));
            }
            if (filterOn.Equals("supplierPublicId", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Supplier != null && x.Supplier.PublicId.ToString().Contains(filterQuery));
            }
        }

        //sort
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "price" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "actualstockquantity" => isAscending
                    ? query.OrderBy(p => p.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity))
                    : query.OrderByDescending(p => p.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity)),
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
            .Where(p => productIds.Contains(p.Id) && p.TenantPublicId == tenantPublicId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.StockEntries)
            .FirstOrDefaultAsync(p => p.PublicId == publicId && p.TenantPublicId == tenantPublicId && p.IsActive, cancellationToken);
    }

    public async Task<List<Product>> GetByPublicIdsAsync(IEnumerable<Guid> publicIds, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(p => publicIds.Contains(p.PublicId) && p.TenantPublicId == tenantPublicId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<(Product Product, int CurrentStock)>> Top5ProductsWithLowStock(Guid tenantPublicId, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .Include(p => p.StockEntries)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.TenantPublicId == tenantPublicId && p.IsActive)
            .Select(p => new
            {
                Product = p,
                CurrentStock = p.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity)
            })
            .Where(x => x.CurrentStock < x.Product.MinimumStockQuantity)
            .OrderBy(x => x.CurrentStock)
            .Take(5)
            .Select(x => new ValueTuple<Product, int>(x.Product, x.CurrentStock))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyByCategoryIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AnyAsync(p => p.CategoryId == id && p.IsActive, cancellationToken);
    }

    public async Task<bool> AnyBySupplierIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
           .AnyAsync(p => p.SupplierId == id && p.IsActive, cancellationToken);
    }
}
