using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public void Delete(Order order)
    {
        _dbContext.Remove(order);
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.Supplier)
            .Where(o => o.TenantPublicId == tenantPublicId)
            .AsQueryable();

        //sort
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "orderdate" => isAscending ? query.OrderBy(p => p.OrderDate) : query.OrderByDescending(p => p.OrderDate),
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

    public async Task<Order?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(o => o.Supplier)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.PublicId == publicId && o.TenantPublicId == tenantPublicId, cancellationToken);
    }

    public async Task<ICollection<Order>> GetLatestOrders(Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders.Include(o => o.Supplier).Where(o => o.TenantPublicId == tenantPublicId).OrderByDescending(o => o.CreatedAt).Take(3).ToListAsync(cancellationToken);
    }

    public async Task<int> GetNumberOfActiveOrders(Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders.Where(o => (o.TenantPublicId == tenantPublicId && (o.OrderStatus == Domain.Enums.OrderStatus.Pending || o.OrderStatus == Domain.Enums.OrderStatus.Confirmed))).CountAsync(cancellationToken);
    }
}
