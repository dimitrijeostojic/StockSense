using Application.Common.Interfaces;
using Application.DashboardManagement.Get;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class GetDashboardRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetDashboardRequestHandler _sut;

    public GetDashboardRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetDashboardRequestHandler(_productRepository, _currentUserAccessor, _orderRepository);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessWithAggregatedCounts()
    {
        _productRepository.CountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(5);
        _productRepository.NumberOfProductsWithLowStock(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(2);
        _productRepository.Top5ProductsWithLowStock(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<(Product, int)>());
        _orderRepository.GetNumberOfActiveOrders(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(3);
        _orderRepository.GetLatestOrders(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<Order>());

        var result = await _sut.Handle(new GetDashboardRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NumberOfProducts.Should().Be(5);
        result.Value.LowStockProducts.Should().Be(2);
        result.Value.NumOfActiveOrders.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WhenNoData_ReturnsZeroCountsAndEmptyCollections()
    {
        _productRepository.CountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(0);
        _productRepository.NumberOfProductsWithLowStock(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(0);
        _productRepository.Top5ProductsWithLowStock(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<(Product, int)>());
        _orderRepository.GetNumberOfActiveOrders(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(0);
        _orderRepository.GetLatestOrders(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<Order>());

        var result = await _sut.Handle(new GetDashboardRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NumberOfProducts.Should().Be(0);
        result.Value.RecentOrders.Should().BeEmpty();
        result.Value.Top5ProductsWithLowStock.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MapsLatestOrdersToDto()
    {
        var order = EntityFactory.CreateOrderWithNavigation();
        _productRepository.CountAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(1);
        _productRepository.NumberOfProductsWithLowStock(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(0);
        _productRepository.Top5ProductsWithLowStock(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<(Product, int)>());
        _orderRepository.GetNumberOfActiveOrders(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(1);
        _orderRepository.GetLatestOrders(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<Order> { order });

        var result = await _sut.Handle(new GetDashboardRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RecentOrders.Should().HaveCount(1);
    }
}
