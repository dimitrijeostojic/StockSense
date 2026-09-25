namespace Application.GoodsReceiptManagement.CreateGoodsReceipt;

public sealed record CreateGoodsReceiptRequestBody(string? Notes, IEnumerable<CreateGoodsReceiptItemDto> Items);
