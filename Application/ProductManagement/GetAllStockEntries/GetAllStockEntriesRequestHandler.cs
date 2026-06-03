using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.GetAllStockEntries;

internal sealed class GetAllStockEntriesRequestHandler(
    IProductRepository productRepository,
    ICurrentUserAccessor currentUserAccessor
    )
    : IRequestHandler<GetAllStockEntriesRequest, TResult<GetAllStockEntriesResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetAllStockEntriesResponse>> Handle(GetAllStockEntriesRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (product == null)
        {
            return TResult<GetAllStockEntriesResponse>.Failure(ApplicationErrors.NotFound);
        }

        var items = product.StockEntries;

        var stockEntryDtos = items.Select(se => new StockEntryDto(se.PublicId, se.Quantity, se.EntryDate, se.Notes, se.StockEntryType, product.PublicId, product.Name)).ToList();

        var response = new GetAllStockEntriesResponse(stockEntryDtos);
        return TResult<GetAllStockEntriesResponse>.Success(response);
    }
}
