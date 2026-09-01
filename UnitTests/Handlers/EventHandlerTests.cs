using Application.OrderManagement.EventHandlers;
using Application.ProductManagement.EventHandlers;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class LowStockDomainEventHandlerTests
{
    private readonly ILogger<LowStockDomainEvent> _logger = Substitute.For<ILogger<LowStockDomainEvent>>();
    private readonly LowStockDomainEventHandler _sut;

    public LowStockDomainEventHandlerTests()
    {
        _sut = new LowStockDomainEventHandler(_logger);
    }

    [Fact]
    public async Task Handle_DoesNotThrow()
    {
        var notification = new LowStockDomainEvent(Guid.NewGuid(), 3);

        var act = async () => await _sut.Handle(notification, CancellationToken.None);

        await act.Should().NotThrowAsync();
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
