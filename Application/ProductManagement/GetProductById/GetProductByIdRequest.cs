using Domain.Core;
using MediatR;

namespace Application.ProductManagement.GetProductById;

public sealed record GetProductByIdRequest(Guid ProductPublicId)
    : IRequest<TResult<GetProductByIdResponse>>;
