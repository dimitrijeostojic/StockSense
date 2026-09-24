using Domain.Enums;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class AnalyticsRepository(ApplicationDbContext dbContext) : IAnalyticsRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<(Guid ProductPublicId, string ProductName, int CurrentStock, int MinimumStock)>> GetCurrentStockPerProductAsync(
        Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Products
            .Include(p => p.StockEntries)
            .Where(p => p.TenantPublicId == tenantPublicId && p.IsActive)
            .ToListAsync(cancellationToken);

        return products.Select(p => (
            p.PublicId,
            p.Name,
            p.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity),
            p.MinimumStockQuantity));
    }

    public async Task<int> GetBelowMinimumStockCountAsync(Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.StockEntries)
            .Where(p => p.TenantPublicId == tenantPublicId && p.IsActive &&
                p.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity) < p.MinimumStockQuantity)
            .CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<(DateTime Date, int InQuantity, int OutQuantity)>> GetStockMovementAsync(
        Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var entries = await _dbContext.StockEntries
            .Include(se => se.Product)
            .Where(se => se.Product!.TenantPublicId == tenantPublicId && se.EntryDate >= from && se.EntryDate <= to)
            .Select(se => new { se.EntryDate, se.StockEntryType, se.Quantity })
            .ToListAsync(cancellationToken);

        return entries
            .GroupBy(e => e.EntryDate.Date)
            .Select(g => (
                g.Key,
                g.Where(e => e.StockEntryType == StockEntryType.In).Sum(e => e.Quantity),
                g.Where(e => e.StockEntryType == StockEntryType.Out).Sum(e => e.Quantity)))
            .OrderBy(x => x.Item1);
    }

    public async Task<(int TotalCount, decimal TotalValue)> GetOrderVolumeAndValueAsync(
        Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.TenantPublicId == tenantPublicId && o.OrderDate >= from && o.OrderDate <= to)
            .ToListAsync(cancellationToken);

        var totalValue = orders.Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity));
        return (orders.Count, totalValue);
    }

    public async Task<IEnumerable<(OrderStatus Status, int Count)>> GetOrderStatusBreakdownAsync(
        Guid tenantPublicId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var breakdown = await _dbContext.Orders
            .Where(o => o.TenantPublicId == tenantPublicId && o.OrderDate >= from && o.OrderDate <= to)
            .GroupBy(o => o.OrderStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return breakdown.Select(x => (x.Status, x.Count));
    }

    public async Task<IEnumerable<(Guid SupplierPublicId, string SupplierName, int OrderCount, decimal TotalValue)>> GetTopSuppliersAsync(
        Guid tenantPublicId, DateTime from, DateTime to, int topN, CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.Supplier)
            .Include(o => o.OrderItems)
            .Where(o => o.TenantPublicId == tenantPublicId && o.OrderDate >= from && o.OrderDate <= to && o.Supplier != null)
            .ToListAsync(cancellationToken);

        return orders
            .GroupBy(o => new { o.Supplier!.PublicId, o.Supplier.Name })
            .Select(g => (
                g.Key.PublicId,
                g.Key.Name,
                g.Count(),
                g.Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity))))
            .OrderByDescending(x => x.Item4)
            .Take(topN);
    }
}
