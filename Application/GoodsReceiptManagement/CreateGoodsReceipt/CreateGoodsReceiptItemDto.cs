namespace Application.GoodsReceiptManagement.CreateGoodsReceipt;

public sealed record CreateGoodsReceiptItemDto(Guid OrderItemPublicId, int ReceivedQuantity);
