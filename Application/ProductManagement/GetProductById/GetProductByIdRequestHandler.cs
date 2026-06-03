using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.GetProductById;

internal sealed class GetProductByIdRequestHandler(
    IProductRepository productRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetProductByIdRequest, TResult<GetProductByIdResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetProductByIdResponse>> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
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
