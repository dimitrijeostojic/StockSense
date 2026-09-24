using Application.Abstractions.Services;
using Application.AnalyticsManagement.Common;
using Application.AnalyticsManagement.GetBusinessAnalytics;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace UnitTests.Handlers;

public sealed class GetBusinessAnalyticsRequestHandlerTests
{
    private readonly IAnalyticsRepository _analyticsRepository = Substitute.For<IAnalyticsRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetBusinessAnalyticsRequestHandler _sut;

    private static readonly DateTime _from = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _to = new(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Guid _tenantId = Guid.NewGuid();

    private static readonly List<(Guid, string, int, int)> _stockItems = new()
    {
        (Guid.NewGuid(), "Widget A", 50, 10),
        (Guid.NewGuid(), "Widget B", 3, 20)
    };

    public GetBusinessAnalyticsRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(_tenantId);
        _sut = new GetBusinessAnalyticsRequestHandler(_analyticsRepository, _currentUserAccessor);

        _analyticsRepository
            .GetCurrentStockPerProductAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((_stockItems, 10));
        _analyticsRepository
            .GetBelowMinimumStockCountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);
        _analyticsRepository
            .GetStockMovementAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<(DateTime, int, int)> { (new DateTime(2026, 5, 1), 100, 40) });
        _analyticsRepository
            .GetOrderVolumeAndValueAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((TotalCount: 12, TotalValue: 4800m));
        _analyticsRepository
            .GetOrderStatusBreakdownAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<(OrderStatus, int)>
            {
                (OrderStatus.Pending, 3),
                (OrderStatus.Received, 9)
            });
        _analyticsRepository
            .GetTopSuppliersAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<(Guid, string, int, decimal)>
            {
                (Guid.NewGuid(), "Acme Corp", 8, 3200m)
            });
    }

    [Fact]
    public async Task Handle_ReturnsSuccess()
    {
        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MapsInventoryMetrics()
    {
        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.Value!.InventoryMetrics.CurrentStockPerProduct.Should().HaveCount(2);
        result.Value.InventoryMetrics.StockTotalCount.Should().Be(10);
        result.Value.InventoryMetrics.BelowMinimumCount.Should().Be(1);
        result.Value.InventoryMetrics.StockMovement.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_MapsOrderMetrics()
    {
        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.Value!.OrderMetrics.TotalCount.Should().Be(12);
        result.Value.OrderMetrics.TotalValue.Should().Be(4800m);
        result.Value.OrderMetrics.StatusBreakdown.Should().HaveCount(2);
        result.Value.OrderMetrics.TopSuppliers.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_MapsOrderStatusToString()
    {
        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.Value!.OrderMetrics.StatusBreakdown.Should().Contain(x => x.Name == "Pending" && x.Count == 3);
        result.Value.OrderMetrics.StatusBreakdown.Should().Contain(x => x.Name == "Received" && x.Count == 9);
    }

    [Fact]
    public async Task Handle_PassesTenantPublicIdToAllQueries()
    {
        await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        await _analyticsRepository.Received(1).GetCurrentStockPerProductAsync(_tenantId, Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _analyticsRepository.Received(1).GetBelowMinimumStockCountAsync(_tenantId, Arg.Any<CancellationToken>());
        await _analyticsRepository.Received(1).GetOrderVolumeAndValueAsync(_tenantId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesStockPaginationToRepository()
    {
        await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to), StockPage: 2, StockPageSize: 5), CancellationToken.None);

        await _analyticsRepository.Received(1).GetCurrentStockPerProductAsync(
            Arg.Any<Guid>(),
            Arg.Is<int>(2),
            Arg.Is<int>(5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesTopNToTopSuppliersQuery()
    {
        await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to), TopN: 3), CancellationToken.None);

        await _analyticsRepository.Received(1).GetTopSuppliersAsync(
            Arg.Any<Guid>(),
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Is<int>(3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoData_ReturnsSuccessWithZeroValues()
    {
        _analyticsRepository
            .GetCurrentStockPerProductAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<(Guid, string, int, int)>(), 0));
        _analyticsRepository
            .GetBelowMinimumStockCountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(0);
        _analyticsRepository
            .GetStockMovementAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(DateTime, int, int)>());
        _analyticsRepository
            .GetOrderVolumeAndValueAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((0, 0m));
        _analyticsRepository
            .GetOrderStatusBreakdownAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(OrderStatus, int)>());
        _analyticsRepository
            .GetTopSuppliersAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(Guid, string, int, decimal)>());

        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.InventoryMetrics.CurrentStockPerProduct.Should().BeEmpty();
        result.Value.InventoryMetrics.StockTotalCount.Should().Be(0);
        result.Value.OrderMetrics.TotalCount.Should().Be(0);
        result.Value.OrderMetrics.TotalValue.Should().Be(0m);
    }
}
