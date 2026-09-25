using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IGoodsReceiptRepository
{
    Task AddAsync(GoodsReceipt goodsReceipt, CancellationToken cancellationToken = default);
    Task<GoodsReceipt?> GetByOrderPublicIdAsync(Guid orderPublicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForOrderAsync(Guid orderPublicId, Guid tenantPublicId, CancellationToken cancellationToken = default);
}
