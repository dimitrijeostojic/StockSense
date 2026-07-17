using Application.Common.Interfaces;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.GetAllProducts;

internal sealed class GetAllProductsRequestHandler(
    IProductRepository productRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetAllProductsRequest, TResult<GetAllProductsResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetAllProductsResponse>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _productRepository.GetAllAsync(_currentUserAccessor.TenantPublicId, request.SearchTerm, request.SortBy, request.IsAscending, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(p => new GetAllProductsDto(
            p.PublicId,
            p.Name,
            p.Description,
            p.Price,
            p.MinimumStockQuantity,
           p.StockEntries.Sum(se => se.StockEntryType == Domain.Enums.StockEntryType.In ? se.Quantity : -se.Quantity),
            p.Category!.PublicId,
            p.Category.Name,
            p.Supplier!.PublicId,
            p.Supplier.Name)
            );

        var response = new GetAllProductsResponse(dtos, totalCount, request.PageNumber, request.PageSize);

        return TResult<GetAllProductsResponse>.Success(response);
    }
}
