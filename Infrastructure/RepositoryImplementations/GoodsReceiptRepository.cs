using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class GoodsReceiptRepository(ApplicationDbContext dbContext) : IGoodsReceiptRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddAsync(GoodsReceipt goodsReceipt, CancellationToken cancellationToken = default)
    {
        await _dbContext.GoodsReceipts.AddAsync(goodsReceipt, cancellationToken);
    }

    public async Task<GoodsReceipt?> GetByOrderPublicIdAsync(Guid orderPublicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GoodsReceipts
            .Include(gr => gr.Items)
                .ThenInclude(i => i.OrderItem)
                    .ThenInclude(oi => oi!.Product)
            .FirstOrDefaultAsync(gr => gr.OrderPublicId == orderPublicId && gr.TenantPublicId == tenantPublicId, cancellationToken);
    }

    public async Task<bool> ExistsForOrderAsync(Guid orderPublicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GoodsReceipts
            .AnyAsync(gr => gr.OrderPublicId == orderPublicId && gr.TenantPublicId == tenantPublicId, cancellationToken);
    }
}
