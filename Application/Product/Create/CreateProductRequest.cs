using Domain.Core;
using MediatR;

namespace Application.Product.Create;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int MinimumStockQuantity,
    Guid CategoryPublicId,
    Guid SupplierPublicId)
    : IRequest<TResult<CreateProductResponse>>;
