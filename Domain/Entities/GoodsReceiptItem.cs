using Domain.Primitives;

namespace Domain.Entities;

public class GoodsReceiptItem : Entity
{
    public int GoodsReceiptId { get; private set; }
    public int OrderItemId { get; private set; }
    public int ProductId { get; private set; }
    public int OrderedQuantity { get; private set; }
    public int ReceivedQuantity { get; private set; }

    public OrderItem? OrderItem { get; private set; }

    private GoodsReceiptItem() { }

    internal static GoodsReceiptItem Create(int orderItemId, int productId, int orderedQuantity, int receivedQuantity)
    {
        return new GoodsReceiptItem
        {
            OrderItemId = orderItemId,
            ProductId = productId,
            OrderedQuantity = orderedQuantity,
            ReceivedQuantity = receivedQuantity
        };
    }
}
