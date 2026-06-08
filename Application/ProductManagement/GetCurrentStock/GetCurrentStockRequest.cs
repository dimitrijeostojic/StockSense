using Domain.Core;
using MediatR;

namespace Application.ProductManagement.GetCurrentStock;

public sealed record GetCurrentStockRequest(Guid ProductPublicId)
    : IRequest<TResult<GetCurrentStockResponse>>;
