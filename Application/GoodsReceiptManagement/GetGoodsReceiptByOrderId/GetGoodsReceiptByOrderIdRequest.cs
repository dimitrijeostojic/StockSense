using Domain.Core;
using MediatR;

namespace Application.GoodsReceiptManagement.GetGoodsReceiptByOrderId;

public sealed record GetGoodsReceiptByOrderIdRequest(Guid OrderPublicId) : IRequest<TResult<GetGoodsReceiptByOrderIdResponse>>;
