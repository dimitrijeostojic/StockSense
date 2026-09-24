using Application.Abstractions.Services;
using Application.AnalyticsManagement.Common;
using Application.AnalyticsManagement.GetUserAnalytics;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace UnitTests.Handlers;

public sealed class GetUserAnalyticsRequestHandlerTests
{
    private readonly IAnalyticsRepository _analyticsRepository = Substitute.For<IAnalyticsRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetUserAnalyticsRequestHandler _sut;

    private static readonly DateTime _from = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _to = new(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc);

    public GetUserAnalyticsRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetUserAnalyticsRequestHandler(_analyticsRepository, _currentUserAccessor);

        _analyticsRepository
            .GetUserIdsByTenantAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<string> { "user-1", "user-2" });
        _analyticsRepository
            .GetActivityByEntityTypeAsync(Arg.Any<List<string>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<(string, int)> { ("Product", 5), ("Order", 3) });
        _analyticsRepository
            .GetActivityByActionTypeAsync(Arg.Any<List<string>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<(string, int)> { ("Added", 4), ("Modified", 4) });
        _analyticsRepository
            .GetTopActiveUsersAsync(Arg.Any<List<string>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<(string, int)> { ("alice@example.com", 10) });
        _analyticsRepository
            .GetRegistrationTrendAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<(string, int)> { ("2026-01", 1), ("2026-02", 1) });
    }

    [Fact]
    public async Task Handle_ReturnsSuccess()
    {
        var result = await _sut.Handle(new GetUserAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MapsAllResponseSections()
    {
        var result = await _sut.Handle(new GetUserAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.Value!.ActivityByEntityType.Should().HaveCount(2);
        result.Value.ActivityByActionType.Should().HaveCount(2);
        result.Value.TopActiveUsers.Should().HaveCount(1);
        result.Value.RegistrationTrend.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_MapsEntityActivityCorrectly()
    {
        var result = await _sut.Handle(new GetUserAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.Value!.ActivityByEntityType[0].EntityName.Should().Be("Product");
        result.Value.ActivityByEntityType[0].Count.Should().Be(5);
    }

    [Fact]
    public async Task Handle_PassesTenantPublicIdToRepository()
    {
        var tenantId = Guid.NewGuid();
        _currentUserAccessor.TenantPublicId.Returns(tenantId);

        await _sut.Handle(new GetUserAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        await _analyticsRepository.Received(1).GetUserIdsByTenantAsync(tenantId, Arg.Any<CancellationToken>());
        await _analyticsRepository.Received(1).GetRegistrationTrendAsync(tenantId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesTopNToRepository()
    {
        await _sut.Handle(new GetUserAnalyticsRequest(new TimeRangeQuery(_from, _to), TopN: 5), CancellationToken.None);

        await _analyticsRepository.Received(1).GetTopActiveUsersAsync(
            Arg.Any<List<string>>(),
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Is<int>(5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoUsers_ReturnsSuccessWithEmptyActivityCollections()
    {
        _analyticsRepository
            .GetUserIdsByTenantAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<string>());
        _analyticsRepository
            .GetActivityByEntityTypeAsync(Arg.Any<List<string>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(string, int)>());
        _analyticsRepository
            .GetActivityByActionTypeAsync(Arg.Any<List<string>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(string, int)>());
        _analyticsRepository
            .GetTopActiveUsersAsync(Arg.Any<List<string>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(string, int)>());
        _analyticsRepository
            .GetRegistrationTrendAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<(string, int)>());

        var result = await _sut.Handle(new GetUserAnalyticsRequest(new TimeRangeQuery(_from, _to)), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ActivityByEntityType.Should().BeEmpty();
        result.Value.TopActiveUsers.Should().BeEmpty();
    }
}
