using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.Common.Options;
using Application.UserManagement.Delete;
using Application.UserManagement.GetAll;
using Application.UserManagement.InviteUser;
using Application.UserManagement.ResendInvite;
using Domain.Abstractions;
using Domain.Dtos;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class DeleteUserRequestHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthUnitOfWork _unitOfWork = Substitute.For<IAuthUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly DeleteUserRequestHandler _sut;

    public DeleteUserRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new DeleteUserRequestHandler(_userRepository, _unitOfWork, _currentUserAccessor, _userManager);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundFailure()
    {
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ApplicationUser?)null);

        var result = await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenLastAdminDeleted_ReturnsCannotDeleteAdminError()
    {
        var user = ApplicationUser.Create("admin", "admin@test.com", "Admin", "User", 1);
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "Admin" });
        _userManager.GetUsersInRoleAsync("Admin").Returns(new List<ApplicationUser> { user });

        var result = await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Contain("CannotDeleteAdminUser");
    }

    [Fact]
    public async Task Handle_WhenNotLastAdmin_AdminCanBeDeleted()
    {
        var user = ApplicationUser.Create("admin", "admin@test.com", "Admin", "User", 1);
        var otherAdmin = ApplicationUser.Create("admin2", "admin2@test.com", "Admin2", "User", 1);
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "Admin" });
        _userManager.GetUsersInRoleAsync("Admin").Returns(new List<ApplicationUser> { user, otherAdmin });

        var result = await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserExists_DeactivatesAndSaves()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "User" });

        var result = await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_DoesNotSave()
    {
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ApplicationUser?)null);

        await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class GetAllUsersRequestHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly GetAllUsersRequestHandler _sut;

    public GetAllUsersRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new GetAllUsersRequestHandler(_userRepository, _currentUserAccessor, _userManager);
    }

    [Fact]
    public async Task Handle_WhenNoUsers_ReturnsSuccessWithEmptyList()
    {
        _userRepository.GetAllUsersAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<ApplicationUser>());

        var result = await _sut.Handle(new GetAllUsersRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenUsersExist_ReturnsMappedDtos()
    {
        var user1 = ApplicationUser.Create("alice", "alice@test.com", "Alice", "Smith", 1);
        var user2 = ApplicationUser.Create("bob", "bob@test.com", "Bob", "Jones", 1);
        _userRepository.GetAllUsersAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<ApplicationUser> { user1, user2 });
        _userManager.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(new List<string> { "User" });

        var result = await _sut.Handle(new GetAllUsersRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }
}

public sealed class InviteUserRequestHandlerTests
{
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly ILogger<InviteUserRequestHandler> _logger = NullLogger<InviteUserRequestHandler>.Instance;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<AppOptions> _options;

    private readonly InviteUserRequestHandler _sut;

    public InviteUserRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _options = Options.Create(new AppOptions { FrontendBaseUrl = "https://app.test" });
        _sut = new InviteUserRequestHandler(_currentUserAccessor, _tenantRepository, _userManager, _emailService, _options, _logger);
    }

    private static InviteUserRequest ValidRequest() => new("Jane", "Doe", "jane@test.com", "User");

    [Fact]
    public async Task Handle_WhenTenantNotFound_ReturnsNotFoundFailure()
    {
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Tenant?)null);

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsEmailAlreadyExistsError()
    {
        var tenant = Tenant.Create("TestCo", "987654321", "Main St 1");
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(tenant);
        _userManager.FindByEmailAsync("jane@test.com")
            .Returns(ApplicationUser.Create("jane", "jane@test.com", "Jane", "Doe", 1));

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenUserCreateFails_ReturnsFailure()
    {
        var tenant = Tenant.Create("TestCo", "987654321", "Main St 1");
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(tenant);
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Create failed" }));

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenAllValid_CreatesUserWithNoPasswordAndReturnsSuccess()
    {
        var tenant = Tenant.Create("TestCo", "987654321", "Main St 1");
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(tenant);
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        _userManager.GenerateUserTokenAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("invite-token");

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        // CreateAsync called without password
        await _userManager.Received(1).CreateAsync(Arg.Is<ApplicationUser>(u => u.Email == "jane@test.com"));
        await _userManager.DidNotReceive().CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenAllValid_AssignsRequestedRole()
    {
        var tenant = Tenant.Create("TestCo", "987654321", "Main St 1");
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(tenant);
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        _userManager.GenerateUserTokenAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("invite-token");

        await _sut.Handle(new InviteUserRequest("Jane", "Doe", "jane@test.com", "Admin"), CancellationToken.None);

        await _userManager.Received(1).AddToRoleAsync(Arg.Any<ApplicationUser>(), "Admin");
    }
}

public sealed class ResendInviteRequestHandlerTests
{
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IOptions<AppOptions> _options = Options.Create(new AppOptions { FrontendBaseUrl = "https://app.test" });
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ResendInviteRequestHandler _sut;

    public ResendInviteRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new ResendInviteRequestHandler(
            _currentUserAccessor, _userRepository, _userManager, _emailService, _options,
            NullLogger<ResendInviteRequestHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ApplicationUser?)null);

        var result = await _sut.Handle(new ResendInviteRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyActive_ReturnsUserAlreadyActiveError()
    {
        var user = ApplicationUser.Create("jane", "jane@test.com", "Jane", "Doe", 1);
        user.EmailConfirmed = true;
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _sut.Handle(new ResendInviteRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.UserAlreadyActive);
    }

    [Fact]
    public async Task Handle_WhenPendingUser_GeneratesTokenAndSendsEmail()
    {
        var user = ApplicationUser.Create("jane", "jane@test.com", "Jane", "Doe", 1);
        // EmailConfirmed defaults to false — pending
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _userManager.GenerateUserTokenAsync(user, Arg.Any<string>(), Arg.Any<string>())
            .Returns("new-token");

        var result = await _sut.Handle(new ResendInviteRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _emailService.Received(1).SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }
}
