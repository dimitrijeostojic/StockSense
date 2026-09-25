using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.Common.Options;
using Application.TenantManagement.CreateTenant;
using Domain.Abstractions;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace UnitTests.Handlers;

public sealed class CreateTenantRequestHandlerTests
{
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthUnitOfWork _authUnitOfWork = Substitute.For<IAuthUnitOfWork>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IOptions<AppOptions> _options = Options.Create(new AppOptions { FrontendBaseUrl = "https://app.test" });

    private readonly CreateTenantRequestHandler _sut;

    public CreateTenantRequestHandlerTests()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _authUnitOfWork.BeginTransaction().Returns(Substitute.For<System.Data.IDbTransaction>());
        _sut = new CreateTenantRequestHandler(
            _tenantRepository, _userManager, _authUnitOfWork, _emailService, _options,
            NullLogger<CreateTenantRequestHandler>.Instance);
    }

    private static CreateTenantRequest ValidRequest() =>
        new("AcmeCorp", "123456789", "Main St 1", "admin@acme.com", "Alice", "Smith");

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsEmailAlreadyExistsError()
    {
        _userManager.FindByEmailAsync("admin@acme.com")
            .Returns(ApplicationUser.Create("admin", "admin@acme.com", "Alice", "Smith", 1));

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenPibAlreadyExists_ReturnsPibAlreadyExistsError()
    {
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _tenantRepository.GetByPibAsync("123456789", Arg.Any<CancellationToken>())
            .Returns(Tenant.Create("OtherCorp", "123456789", "Other St"));

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.PIBAlreadyExists);
    }

    [Fact]
    public async Task Handle_WhenValid_CreatesTenantAndPendingAdminAndReturnsSuccess()
    {
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);
        _tenantRepository.GetByPibAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Tenant?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        _userManager.GenerateUserTokenAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("invite-token");

        var result = await _sut.Handle(ValidRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TenantName.Should().Be("AcmeCorp");
        result.Value.AdminEmail.Should().Be("admin@acme.com");
        await _tenantRepository.Received(1).AddAsync(Arg.Any<Tenant>(), Arg.Any<CancellationToken>());
        // Admin created without password
        await _userManager.Received(1).CreateAsync(Arg.Is<ApplicationUser>(u => u.Email == "admin@acme.com"));
        await _userManager.DidNotReceive().CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }
}
