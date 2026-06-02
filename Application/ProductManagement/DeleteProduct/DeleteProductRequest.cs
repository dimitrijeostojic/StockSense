using Domain.Core;
using MediatR;

namespace Application.ProductManagement.DeleteProduct;

public sealed record DeleteProductRequest(Guid PublicId)
    : IRequest<Result>;
