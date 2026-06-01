using Domain.Core;
using MediatR;

namespace Application.Product.Delete;

public sealed record DeleteProductRequest(Guid PublicId)
    : IRequest<Result>;
