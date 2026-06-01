using Domain.Core;
using MediatR;

namespace Application.Product.GetById;

public sealed record GetProductByIdRequest(Guid ProductPublicId)
    : IRequest<TResult<GetProductByIdResponse>>;
