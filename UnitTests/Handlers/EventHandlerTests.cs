using Application.Abstractions.Services;
using Application.OrderManagement.EventHandlers;
using Application.ProductManagement.EventHandlers;
using Domain.Abstractions;
using Domain.Dtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reflection;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class LowStockDomainEventHandlerTests
{
    private readonly ILogger<LowStockDomainEvent> _logger = Substitute.For<ILogger<LowStockDomainEvent>>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly UserManager<ApplicationUser> _userManager = Substitute.For<UserManager<ApplicationUser>>(
        Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
    private readonly ITenantRepository _tenantRepository = Substitute.For<ITenantRepository>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly LowStockDomainEventHandler _sut;

    public LowStockDomainEventHandlerTests()
    {
        _sut = new LowStockDomainEventHandler(_logger, _emailService, _userManager, _tenantRepository, _productRepository);
    }

    [Fact]
    public async Task Handle_WhenTenantNotFound_DoesNotSendEmail()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns((Tenant?)null);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.DidNotReceive().SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTenantHasNoUsers_DoesNotSendEmail()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var tenant = Tenant.Create("Test Tenant", "123456789", "Test Address");
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.DidNotReceive().SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_DoesNotSendEmail()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var user = ApplicationUser.Create("user1", "user1@test.com", "John", "Doe", 1);
        var tenant = CreateTenantWithUsers(user);
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);
        _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.DidNotReceive().SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserHasNoEmail_SkipsUser()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var user = ApplicationUser.Create("user1", "", "John", "Doe", 1);
        var tenant = CreateTenantWithUsers(user);
        var product = EntityFactory.CreateProductWithNavigation();
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);
        _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.DidNotReceive().SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSingleUserWithEmail_SendsOneEmail()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var user = ApplicationUser.Create("user1", "user1@test.com", "John", "Doe", 1);
        var tenant = CreateTenantWithUsers(user);
        var product = EntityFactory.CreateProductWithNavigation();
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);
        _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.Received(1).SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSingleUserWithEmail_SendsToCorrectRecipient()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var user = ApplicationUser.Create("user1", "user1@test.com", "John", "Doe", 1);
        var tenant = CreateTenantWithUsers(user);
        var product = EntityFactory.CreateProductWithNavigation();
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);
        _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.Received(1).SendAsync(
            Arg.Is<EmailMessageDto>(m => m.To == "user1@test.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMultipleUsers_SendsEmailToEach()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var user1 = ApplicationUser.Create("user1", "user1@test.com", "John", "Doe", 1);
        var user2 = ApplicationUser.Create("user2", "user2@test.com", "Jane", "Doe", 1);
        var tenant = CreateTenantWithUsers(user1, user2);
        var product = EntityFactory.CreateProductWithNavigation();
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);
        _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.Received(2).SendAsync(Arg.Any<EmailMessageDto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSomeUsersHaveNoEmail_SendsOnlyToUsersWithEmail()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), Guid.NewGuid(), 3);
        var userWithEmail = ApplicationUser.Create("user1", "user1@test.com", "John", "Doe", 1);
        var userWithoutEmail = ApplicationUser.Create("user2", "", "Jane", "Doe", 1);
        var tenant = CreateTenantWithUsers(userWithEmail, userWithoutEmail);
        var product = EntityFactory.CreateProductWithNavigation();
        _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(tenant);
        _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(notification, CancellationToken.None);

        await _emailService.Received(1).SendAsync(
            Arg.Is<EmailMessageDto>(m => m.To == "user1@test.com"),
            Arg.Any<CancellationToken>());
    }

    private static Tenant CreateTenantWithUsers(params ApplicationUser[] users)
    {
        var tenant = Tenant.Create("Test Tenant", "123456789", "Test Address");
        var field = typeof(Tenant).GetField("_applicationUsers", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var list = (List<ApplicationUser>)field.GetValue(tenant)!;
        list.AddRange(users);
        return tenant;
    }

}

public sealed class OrderReceivedDomainEventHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly OrderReceivedDomainEventHandler _sut;

    public OrderReceivedDomainEventHandlerTests()
    {
        _sut = new OrderReceivedDomainEventHandler(_productRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenProductsFound_AddsStockEntriesAndSaves()
    {
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        var productId = product.Id;

        _productRepository.GetByIdsAsync(
            Arg.Any<List<int>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });

        var orderItems = new List<OrderReceivedItem> { new(productId, 5) };
        var notification = new OrderReceivedDomainEvent(Guid.NewGuid(), Guid.NewGuid(), orderItems);

        await _sut.Handle(notification, CancellationToken.None);

        product.StockEntries.Should().HaveCount(1);
        product.StockEntries.First().Quantity.Should().Be(5);
        product.StockEntries.First().StockEntryType.Should().Be(StockEntryType.In);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductNotFoundForOrderItem_Throws()
    {
        _productRepository.GetByIdsAsync(
            Arg.Any<List<int>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product>());

        var orderItems = new List<OrderReceivedItem> { new(999, 3) };
        var notification = new OrderReceivedDomainEvent(Guid.NewGuid(), Guid.NewGuid(), orderItems);

        var act = async () => await _sut.Handle(notification, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Product not found");
    }

    [Fact]
    public async Task Handle_WithMultipleProducts_AddsStockEntryForEach()
    {
        var product1 = EntityFactory.CreateProduct("P1", minimumStock: 0);
        var product2 = EntityFactory.CreateProduct("P2", minimumStock: 0);
        product1.Id = 1;
        product2.Id = 2;

        _productRepository.GetByIdsAsync(
            Arg.Any<List<int>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product1, product2 });

        var orderItems = new List<OrderReceivedItem>
        {
            new(product1.Id, 10),
            new(product2.Id, 20)
        };
        var notification = new OrderReceivedDomainEvent(Guid.NewGuid(), Guid.NewGuid(), orderItems);

        await _sut.Handle(notification, CancellationToken.None);

        product1.StockEntries.Should().HaveCount(1);
        product2.StockEntries.Should().HaveCount(1);
        product1.StockEntries.First().Quantity.Should().Be(10);
        product2.StockEntries.First().Quantity.Should().Be(20);
    }
}
