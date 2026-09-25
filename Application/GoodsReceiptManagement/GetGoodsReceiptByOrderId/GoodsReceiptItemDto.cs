namespace Application.GoodsReceiptManagement.GetGoodsReceiptByOrderId;

public sealed record GoodsReceiptItemDto(
    Guid ProductPublicId,
    string ProductName,
    string Sku,
    int OrderedQuantity,
    int ReceivedQuantity);
