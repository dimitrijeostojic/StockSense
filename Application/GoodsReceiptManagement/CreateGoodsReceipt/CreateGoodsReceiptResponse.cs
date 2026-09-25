namespace Application.GoodsReceiptManagement.CreateGoodsReceipt;

public sealed record CreateGoodsReceiptResponse(Guid PublicId, Guid OrderPublicId, DateTime ReceivedAt);
