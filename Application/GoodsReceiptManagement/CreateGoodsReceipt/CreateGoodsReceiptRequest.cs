using Domain.Core;
using MediatR;

namespace Application.GoodsReceiptManagement.CreateGoodsReceipt;

public sealed record CreateGoodsReceiptRequest(
    Guid OrderPublicId,
    string? Notes,
    IEnumerable<CreateGoodsReceiptItemDto> Items) : IRequest<TResult<CreateGoodsReceiptResponse>>;
