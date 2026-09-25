using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.TenantManagement.GetMyTenant;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace UnitTests.Handlers;

public sealed class GetMyTenantWithOnboardingFlagTests
{
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly GetMyTenantRequestHandler _sut;

    public GetMyTenantWithOnboardingFlagTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _currentUserAccessor.UserId.Returns("user-id-1");
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new GetMyTenantRequestHandler(_tenantRepository, _currentUserAccessor, _userManager);
    }

    [Fact]
    public async Task Handle_WhenTenantNotFound_ReturnsNotFoundError()
    {
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Tenant?)null);

        var result = await _sut.Handle(new GetMyTenantRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserHasNotSeenAnyTour_ReturnsEmptySeenTourPages()
    {
        var tenant = Tenant.Create("TestCo", "123456789", "Street 1");
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(tenant);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        var result = await _sut.Handle(new GetMyTenantRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SeenTourPages.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenUserHasSeenDashboardTour_ReturnsDashboardInSeenTourPages()
    {
        var tenant = Tenant.Create("TestCo", "123456789", "Street 1");
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        user.CompleteTour("dashboard");
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(tenant);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        var result = await _sut.Handle(new GetMyTenantRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SeenTourPages.Should().Contain("dashboard");
    }
}
