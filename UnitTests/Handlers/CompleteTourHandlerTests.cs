using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.UserManagement.CompleteTour;
using Domain.Abstractions;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace UnitTests.Handlers;

public sealed class CompleteTourRequestHandlerTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthUnitOfWork _unitOfWork = Substitute.For<IAuthUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly CompleteTourRequestHandler _sut;

    public CompleteTourRequestHandlerTests()
    {
        _currentUserAccessor.UserId.Returns("user-id-1");
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new CompleteTourRequestHandler(_userManager, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WithInvalidPageName_ReturnsBadRequestError()
    {
        var result = await _sut.Handle(new CompleteTourRequest("invalid-page"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidPageName);
    }

    [Fact]
    public async Task Handle_WithInvalidPageName_DoesNotQueryDatabase()
    {
        await _sut.Handle(new CompleteTourRequest("invalid-page"), CancellationToken.None);

        await _userManager.DidNotReceive().FindByIdAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        _userManager.FindByIdAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);

        var result = await _sut.Handle(new CompleteTourRequest("products"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_DoesNotSave()
    {
        _userManager.FindByIdAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);

        await _sut.Handle(new CompleteTourRequest("products"), CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("dashboard")]
    [InlineData("products")]
    [InlineData("suppliers")]
    [InlineData("orders")]
    [InlineData("categories")]
    [InlineData("users")]
    public async Task Handle_WithValidPageName_AddsPageAndSaves(string pageName)
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        var result = await _sut.Handle(new CompleteTourRequest(pageName), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.GetSeenTourPages().Should().Contain(pageName);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CalledTwiceWithSamePage_IsIdempotent()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userManager.FindByIdAsync("user-id-1").Returns(user);

        await _sut.Handle(new CompleteTourRequest("products"), CancellationToken.None);
        await _sut.Handle(new CompleteTourRequest("products"), CancellationToken.None);

        user.GetSeenTourPages().Should().ContainSingle().Which.Should().Be("products");
    }
}
