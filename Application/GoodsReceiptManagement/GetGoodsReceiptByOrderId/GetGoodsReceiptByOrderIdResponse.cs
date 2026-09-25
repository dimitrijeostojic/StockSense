namespace Application.GoodsReceiptManagement.GetGoodsReceiptByOrderId;

public sealed record GetGoodsReceiptByOrderIdResponse(
    Guid PublicId,
    Guid OrderPublicId,
    DateTime ReceivedAt,
    string? Notes,
    IEnumerable<GoodsReceiptItemDto> Items);
