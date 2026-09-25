using Domain.Primitives;

namespace Domain.Entities;

public class GoodsReceipt : Entity
{
    public int OrderId { get; private set; }
    public Guid OrderPublicId { get; private set; }
    public Guid TenantPublicId { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<GoodsReceiptItem> Items => _items;

    private readonly List<GoodsReceiptItem> _items = [];

    private GoodsReceipt() { }

    public static GoodsReceipt Create(int orderId, Guid orderPublicId, Guid tenantPublicId, string? notes)
    {
        return new GoodsReceipt
        {
            OrderId = orderId,
            OrderPublicId = orderPublicId,
            TenantPublicId = tenantPublicId,
            ReceivedAt = DateTime.UtcNow,
            Notes = notes
        };
    }

    public void AddItem(int orderItemId, int productId, int orderedQuantity, int receivedQuantity)
    {
        _items.Add(GoodsReceiptItem.Create(orderItemId, productId, orderedQuantity, receivedQuantity));
    }
}
