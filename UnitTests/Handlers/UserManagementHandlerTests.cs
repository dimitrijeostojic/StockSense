using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.UserManagement.Delete;
using Application.UserManagement.GetAll;
using Application.UserManagement.RegisterUser;
using Domain.Abstractions;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    public async Task Handle_WhenUserIsAdmin_ReturnsCannotDeleteAdminError()
    {
        var user = ApplicationUser.Create("admin", "admin@test.com", "Admin", "User", 1);
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "Admin" });

        var result = await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Contain("CannotDeleteAdminUser");
    }

    [Fact]
    public async Task Handle_WhenUserExists_DeletesAndSaves()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "User" });

        var result = await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _userRepository.Received(1).Delete(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_DoesNotDeleteOrSave()
    {
        _userRepository.GetUserByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ApplicationUser?)null);

        await _sut.Handle(
            new DeleteUserRequest { UserPublicId = Guid.NewGuid() }, CancellationToken.None);

        _userRepository.DidNotReceive().Delete(Arg.Any<ApplicationUser>());
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

public sealed class RegisterUserRequestHandlerTests
{
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<RegisterUserRequestHandler> _logger = NullLogger<RegisterUserRequestHandler>.Instance;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RegisterUserRequestHandler _sut;

    public RegisterUserRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new RegisterUserRequestHandler(_currentUserAccessor, _tenantRepository, _userManager, _emailService, _unitOfWork, _logger);
    }

    private static RegisterUserRequest ValidRequest() => new(
        "Jane", "Doe", "janedoe", "jane@test.com", "Password1!");

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
    public async Task Handle_WhenEmailAlreadyExistsInTenant_ReturnsEmailAlreadyExistsError()
    {
        var tenant = Tenant.Create("TestCo", "987654321", "Main St 1");
        var existingUser = ApplicationUser.Create("existing", "jane@test.com", "Jane", "Doe", tenant.Id);
        var userList = EntityFactory.GetPrivateField<List<ApplicationUser>>(tenant, "_applicationUsers");
        userList.Add(existingUser);
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(tenant);

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
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenAllValid_ReturnsSuccess()
    {
        var tenant = Tenant.Create("TestCo", "987654321", "Main St 1");
        _tenantRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(tenant);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
