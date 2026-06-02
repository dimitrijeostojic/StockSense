using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.GetAllStockEntries;

internal sealed class GetAllStockEntriesRequestHandler(
    IProductRepository productRepository
    )
    : IRequestHandler<GetAllStockEntriesRequest, TResult<GetAllStockEntriesResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    public async Task<TResult<GetAllStockEntriesResponse>> Handle(GetAllStockEntriesRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, cancellationToken);
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
