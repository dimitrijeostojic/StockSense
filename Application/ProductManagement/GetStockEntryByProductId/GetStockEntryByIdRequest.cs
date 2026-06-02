using Domain.Core;
using MediatR;

namespace Application.ProductManagement.GetStockEntryByProductId;

public sealed record GetStockEntryByIdRequest(Guid ProductPublicId, Guid StockEntryPublicId)
    : IRequest<TResult<GetStockEntryByIdResponse>>;