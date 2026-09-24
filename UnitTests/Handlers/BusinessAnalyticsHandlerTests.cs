using Application.Abstractions.Services;
using Application.AnalyticsManagement.Common;
using Application.AnalyticsManagement.GetBusinessAnalytics;
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

    public GetBusinessAnalyticsRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(_tenantId);
        _sut = new GetBusinessAnalyticsRequestHandler(_analyticsRepository, _currentUserAccessor);

        _analyticsRepository
            .GetStockMovementAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<(DateTime, int, int)> { (new DateTime(2026, 5, 1), 100, 40) });
        _analyticsRepository
            .GetTotalOrderValueAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(4800m);
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

        result.Value!.InventoryMetrics.StockMovement.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_MapsOrderMetrics()
    {
        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.Value!.OrderMetrics.TotalValue.Should().Be(4800m);
        result.Value.OrderMetrics.TopSuppliers.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_PassesTenantPublicIdToAllQueries()
    {
        await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        await _analyticsRepository.Received(1).GetTotalOrderValueAsync(_tenantId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
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
            .GetStockMovementAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(DateTime, int, int)>());
        _analyticsRepository
            .GetTotalOrderValueAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(0m);
        _analyticsRepository
            .GetTopSuppliersAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(Guid, string, int, decimal)>());

        var result = await _sut.Handle(new GetBusinessAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.InventoryMetrics.StockMovement.Should().BeEmpty();
        result.Value.OrderMetrics.TotalValue.Should().Be(0m);
    }
}
