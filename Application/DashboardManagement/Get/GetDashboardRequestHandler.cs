using Application.Abstractions.Services;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.DashboardManagement.Get;

internal sealed class GetDashboardRequestHandler(
    IProductRepository productRepository,
    ICurrentUserAccessor currentUserAccessor,
    IOrderRepository orderRepository,
    IDashboardRepository dashboardRepository
    )
    : IRequestHandler<GetDashboardRequest, TResult<GetDashboardResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IDashboardRepository _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));

    public async Task<TResult<GetDashboardResponse>> Handle(GetDashboardRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;
        var numberOfProducts = await _productRepository.CountAsync(tenantPublicId, cancellationToken);
        var numberOfProductsWithLowStock = await _dashboardRepository.GetBelowMinimumStockCountAsync(tenantPublicId, cancellationToken);
        var numberOfActiveOrders = await _orderRepository.GetNumberOfActiveOrders(tenantPublicId, cancellationToken);
        var latestOrders = await _orderRepository.GetLatestOrders(tenantPublicId, cancellationToken);
        var top5ProductWithLowStock = await _productRepository.Top5ProductsWithLowStock(tenantPublicId, cancellationToken);

        var latestOrdersDtos = latestOrders.Select(lo => new DashboardOrderDto(lo.PublicId, lo.Supplier!.Name, lo.OrderDate, lo.OrderStatus)).ToList();

        var top5ProductWithLowStockDtos = top5ProductWithLowStock.Select(p => new DashboardProductDto(p.Product.PublicId, p.Product.Name, p.Product.Price, p.Product.MinimumStockQuantity, p.Product.Category!.Name, p.Product.Supplier!.Name, p.CurrentStock)).ToList();
        var response = new GetDashboardResponse(numberOfProducts, numberOfProductsWithLowStock, numberOfActiveOrders, latestOrdersDtos, top5ProductWithLowStockDtos);

        return TResult<GetDashboardResponse>.Success(response);
    }
}
