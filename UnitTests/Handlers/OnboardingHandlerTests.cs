using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.TenantManagement.CompleteOnboarding;
using Application.TenantManagement.GetMyTenant;
using Domain.Abstractions;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace UnitTests.Handlers;

public sealed class CompleteOnboardingRequestHandlerTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthUnitOfWork _unitOfWork = Substitute.For<IAuthUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly CompleteOnboardingRequestHandler _sut;

    public CompleteOnboardingRequestHandlerTests()
    {
        _currentUserAccessor.UserId.Returns("user-id-1");
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new CompleteOnboardingRequestHandler(_userManager, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        _userManager.FindByIdAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);

        var result = await _sut.Handle(new CompleteOnboardingRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserFound_SetsHasSeenOnboardingAndSaves()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        var result = await _sut.Handle(new CompleteOnboardingRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.HasSeenOnboarding.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_DoesNotSave()
    {
        _userManager.FindByIdAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);

        await _sut.Handle(new CompleteOnboardingRequest(), CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

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
    public async Task Handle_WhenUserHasNotSeenOnboarding_ReturnsHasSeenOnboardingFalse()
    {
        var tenant = Tenant.Create("TestCo", "123456789", "Street 1");
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(tenant);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        var result = await _sut.Handle(new GetMyTenantRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasSeenOnboarding.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenUserHasSeenOnboarding_ReturnsHasSeenOnboardingTrue()
    {
        var tenant = Tenant.Create("TestCo", "123456789", "Street 1");
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        user.CompleteOnboarding();
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(tenant);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        var result = await _sut.Handle(new GetMyTenantRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasSeenOnboarding.Should().BeTrue();
    }
}
