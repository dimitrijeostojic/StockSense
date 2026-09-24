using Application.Abstractions.Services;
using Application.AnalyticsManagement.Common;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.AnalyticsManagement.GetBusinessAnalytics;

internal sealed class GetBusinessAnalyticsRequestHandler(
    IAnalyticsRepository analyticsRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetBusinessAnalyticsRequest, TResult<GetBusinessAnalyticsResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository = analyticsRepository ?? throw new ArgumentNullException(nameof(analyticsRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetBusinessAnalyticsResponse>> Handle(GetBusinessAnalyticsRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;
        var (from, to) = request.TimeRange;

        var currentStock = await _analyticsRepository.GetCurrentStockPerProductAsync(tenantPublicId, cancellationToken);
        var belowMinimum = await _analyticsRepository.GetBelowMinimumStockCountAsync(tenantPublicId, cancellationToken);
        var stockMovement = await _analyticsRepository.GetStockMovementAsync(tenantPublicId, from, to, cancellationToken);

        var (totalCount, totalValue) = await _analyticsRepository.GetOrderVolumeAndValueAsync(tenantPublicId, from, to, cancellationToken);
        var statusBreakdown = await _analyticsRepository.GetOrderStatusBreakdownAsync(tenantPublicId, from, to, cancellationToken);
        var topSuppliers = await _analyticsRepository.GetTopSuppliersAsync(tenantPublicId, from, to, request.TopN, cancellationToken);

        var inventory = new InventoryMetricsDto(
            currentStock.Select(x => new StockLevelDto(x.ProductPublicId, x.ProductName, x.CurrentStock, x.MinimumStock)).ToList(),
            belowMinimum,
            stockMovement.Select(x => new StockMovementPointDto(x.Date, x.InQuantity, x.OutQuantity)).ToList());

        var orders = new OrderMetricsDto(
            totalCount,
            totalValue,
            statusBreakdown.Select(x => new NamedCountDto(x.Status.ToString(), x.Count)).ToList(),
            topSuppliers.Select(x => new TopSupplierDto(x.SupplierPublicId, x.SupplierName, x.OrderCount, x.TotalValue)).ToList());

        return TResult<GetBusinessAnalyticsResponse>.Success(new GetBusinessAnalyticsResponse(inventory, orders));
    }
}
