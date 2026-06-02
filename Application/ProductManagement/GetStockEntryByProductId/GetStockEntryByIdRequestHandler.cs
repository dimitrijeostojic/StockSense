using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.GetStockEntryByProductId;

internal sealed class GetStockEntryByIdRequestHandler(
    IProductRepository productRepository)
    : IRequestHandler<GetStockEntryByIdRequest, TResult<GetStockEntryByIdResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<TResult<GetStockEntryByIdResponse>> Handle(GetStockEntryByIdRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, cancellationToken);
        if (product == null)
        {
            return TResult<GetStockEntryByIdResponse>.Failure(ApplicationErrors.NotFound);
        }

        var stockEntry = product.StockEntries.FirstOrDefault(se => se.PublicId == request.StockEntryPublicId);
        if (stockEntry == null)
        {
            return TResult<GetStockEntryByIdResponse>.Failure(ApplicationErrors.NotFound);
        }

        var response = new GetStockEntryByIdResponse(
            stockEntry.PublicId,
            stockEntry.Quantity,
            stockEntry.EntryDate,
            stockEntry.Notes,
            stockEntry.StockEntryType,
            product.PublicId,
            product.Name
        );

        return TResult<GetStockEntryByIdResponse>.Success(response);
    }
}