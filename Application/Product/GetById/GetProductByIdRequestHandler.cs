using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Product.GetById;

internal sealed class GetProductByIdRequestHandler(
    IProductRepository productRepository)
    : IRequestHandler<GetProductByIdRequest, TResult<GetProductByIdResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<TResult<GetProductByIdResponse>> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, cancellationToken);
        if (product == null)
        {
            return TResult<GetProductByIdResponse>.Failure(ApplicationErrors.NotFound);
        }

        var response = new GetProductByIdResponse(
            product.PublicId,
            product.Name,
            product.Description,
            product.Price,
            product.MinimumStockQuantity,
            product.Category!.PublicId,
            product.Category.Name,
            product.Supplier!.PublicId,
            product.Supplier.Name);

        return TResult<GetProductByIdResponse>.Success(response);
    }
}
