using Domain.Core;
using MediatR;

namespace Application.ProductManagement.GetAllProducts;

public sealed class GetAllProductsRequest
    : PagedRequest, IRequest<TResult<GetAllProductsResponse>>;