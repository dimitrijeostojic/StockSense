using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Core;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.GetCurrentStock;

internal sealed class GetCurrentStockRequestHandler(
    IProductRepository productRepository,
    ICurrentUserAccessor currentUserAccessor
    )
    : IRequestHandler<GetCurrentStockRequest, TResult<GetCurrentStockResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetCurrentStockResponse>> Handle(GetCurrentStockRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (product == null)
        {
            return TResult<GetCurrentStockResponse>.Failure(ApplicationErrors.NotFound);
        }

        var currentStock = product.StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity);

        return TResult<GetCurrentStockResponse>.Success(new GetCurrentStockResponse(currentStock));
    }
}
