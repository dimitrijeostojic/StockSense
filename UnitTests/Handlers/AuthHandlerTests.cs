using Application.Abstractions.Services;
using Application.AuthManagement.Login;
using Application.AuthManagement.Logout;
using Application.AuthManagement.RefreshToken;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class LoginRequestHandlerTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IAuthUnitOfWork _authUnitOfWork = Substitute.For<IAuthUnitOfWork>();
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();

    private readonly LoginRequestHandler _sut;

    public LoginRequestHandlerTests()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);

        _sut = new LoginRequestHandler(
            _userManager, _jwtTokenService, _refreshTokenRepository,
            _authUnitOfWork, _tenantRepository);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsInvalidCredentials()
    {
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);

        var result = await _sut.Handle(
            new LoginRequest("notfound@test.com", "Password1!"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WhenPasswordWrong_ReturnsInvalidCredentials()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userManager.FindByEmailAsync("john@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, Arg.Any<string>()).Returns(false);

        var result = await _sut.Handle(
            new LoginRequest("john@test.com", "WrongPassword"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WhenTenantNotFound_ReturnsNotFoundFailure()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userManager.FindByEmailAsync("john@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Password1!").Returns(true);
        _tenantRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Tenant?)null);

        var result = await _sut.Handle(
            new LoginRequest("john@test.com", "Password1!"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAndTenantValid_ReturnsTokens()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        var tenant = Tenant.Create("TestCo", "123456789", "Street 1");
        _userManager.FindByEmailAsync("john@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Password1!").Returns(true);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "Admin" });
        _tenantRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(tenant);
        _jwtTokenService.GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>())
            .Returns("access-token");

        var result = await _sut.Handle(
            new LoginRequest("john@test.com", "Password1!"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenLoginSucceeds_SavesRefreshToken()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        var tenant = Tenant.Create("TestCo", "123456789", "Street 1");
        _userManager.FindByEmailAsync("john@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Password1!").Returns(true);
        _userManager.GetRolesAsync(user).Returns(new List<string>());
        _tenantRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(tenant);
        _jwtTokenService.GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>())
            .Returns("token");

        await _sut.Handle(new LoginRequest("john@test.com", "Password1!"), CancellationToken.None);

        await _refreshTokenRepository.Received(1).AddAsync(Arg.Any<RefreshToken>(), Arg.Any<CancellationToken>());
        await _authUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class LogoutRequestHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly LogoutRequestHandler _sut;

    public LogoutRequestHandlerTests()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);

        _sut = new LogoutRequestHandler(_refreshTokenRepository, _userManager, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenTokenNotFound_ReturnsInvalidRefreshToken()
    {
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((RefreshToken?)null);

        var result = await _sut.Handle(
            new LogoutRequest { RefreshToken = "nonexistent" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_WhenTokenRevoked_ReturnsInvalidRefreshToken()
    {
        var token = RefreshToken.Create("user-id");
        token.Revoke();
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);

        var result = await _sut.Handle(
            new LogoutRequest { RefreshToken = token.Token! }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var token = RefreshToken.Create("user-id");
        // User navigation property is null by default (not set via EF)
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);

        var result = await _sut.Handle(
            new LogoutRequest { RefreshToken = token.Token! }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserLockedOut_ReturnsUserLockedOut()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        var token = RefreshToken.Create(user.Id);
        EntityFactory.SetPrivate(token, "User", user);
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);
        _userManager.IsLockedOutAsync(user).Returns(true);

        var result = await _sut.Handle(
            new LogoutRequest { RefreshToken = token.Token! }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.UserLockedOut);
    }

    [Fact]
    public async Task Handle_WhenTokenValidAndUserNotLocked_RevokesTokenAndReturnsSuccess()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        var token = RefreshToken.Create(user.Id);
        EntityFactory.SetPrivate(token, "User", user);
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);
        _userManager.IsLockedOutAsync(user).Returns(false);

        var result = await _sut.Handle(
            new LogoutRequest { RefreshToken = token.Token! }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        token.IsRevoked.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}

public sealed class RefreshTokenRequestHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IAuthUnitOfWork _authUnitOfWork = Substitute.For<IAuthUnitOfWork>();
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();

    private readonly RefreshTokenRequestHandler _sut;

    public RefreshTokenRequestHandlerTests()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);

        var transaction = Substitute.For<System.Data.IDbTransaction>();
        _authUnitOfWork.BeginTransaction().Returns(transaction);

        _sut = new RefreshTokenRequestHandler(
            _refreshTokenRepository, _authUnitOfWork, _userManager,
            _jwtTokenService, _tenantRepository);
    }

    [Fact]
    public async Task Handle_WhenTokenNotFound_ReturnsInvalidRefreshToken()
    {
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((RefreshToken?)null);

        var result = await _sut.Handle(
            new RefreshTokenRequest("nonexistent"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_WhenTokenRevoked_ReturnsInvalidRefreshToken()
    {
        var token = RefreshToken.Create("user-id");
        token.Revoke();
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);

        var result = await _sut.Handle(
            new RefreshTokenRequest(token.Token!), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_WhenUserIsNull_ReturnsNotFound()
    {
        var token = RefreshToken.Create("user-id");
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);

        var result = await _sut.Handle(
            new RefreshTokenRequest(token.Token!), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserLockedOut_ReturnsUserLockedOut()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        var token = RefreshToken.Create(user.Id);
        EntityFactory.SetPrivate(token, "User", user);
        _refreshTokenRepository.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(token);
        _userManager.IsLockedOutAsync(user).Returns(true);

        var result = await _sut.Handle(
            new RefreshTokenRequest(token.Token!), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.UserLockedOut);
    }
}
