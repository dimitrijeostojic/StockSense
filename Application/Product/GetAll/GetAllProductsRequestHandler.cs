using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Product.GetAll;

internal sealed class GetAllProductsRequestHandler(
    IProductRepository productRepository)
    : IRequestHandler<GetAllProductsRequest, TResult<GetAllProductsResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<TResult<GetAllProductsResponse>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _productRepository.GetAllAsync(request.SearchTerm, request.SortBy, request.IsAscending, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(p => new GetAllProductsDto(
            p.PublicId,
            p.Name,
            p.Description,
            p.Price,
            p.MinimumStockQuantity,
            p.Category!.PublicId,
            p.Category.Name,
            p.Supplier!.PublicId,
            p.Supplier.Name)
            );

        var response = new GetAllProductsResponse(dtos, totalCount, request.PageNumber, request.PageSize);

        return TResult<GetAllProductsResponse>.Success(response);
    }
}
