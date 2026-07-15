using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.OrderManagement.CreateOrder;
using Application.OrderManagement.DeleteOrder;
using Application.OrderManagement.GetAllOrders;
using Application.OrderManagement.UpdateOrder;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;
using CreateOrderItemDto = Application.OrderManagement.CreateOrder.OrderItemDto;
using GetOrderByIdRequest = Application.OrderManagement.GetOrderById.GetOrderByIdRequest;
using GetOrderByIdRequestHandler = Application.OrderManagement.GetOrderById.GetOrderByIdRequestHandler;

namespace UnitTests.Handlers;

public sealed class CreateOrderRequestHandlerTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly CreateOrderRequestHandler _sut;

    public CreateOrderRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new CreateOrderRequestHandler(
            _orderRepository, _supplierRepository, _productRepository,
            _unitOfWork, _currentUserAccessor);
    }

    private CreateOrderRequest ValidRequest(Guid supplierPublicId, List<CreateOrderItemDto> items) => new(
        supplierPublicId, DateTime.UtcNow.AddDays(-1), null, items);

    [Fact]
    public async Task Handle_WhenSupplierAndProductsExist_ReturnsSuccess()
    {
        var supplierPublicId = Guid.NewGuid();
        var supplier = EntityFactory.CreateSupplier();
        var product = EntityFactory.CreateProduct();

        _supplierRepository.GetByPublicIdAsync(supplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(supplier);
        _productRepository.GetByPublicIdsAsync(
            Arg.Any<IEnumerable<Guid>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns([product]);

        var items = new List<CreateOrderItemDto> { new(product.PublicId, 2) };
        var request = ValidRequest(supplierPublicId, items);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task Handle_WhenSupplierAndProductsExist_AddsOrderAndSaves()
    {
        var supplierPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct();
        _supplierRepository.GetByPublicIdAsync(supplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());
        _productRepository.GetByPublicIdsAsync(
            Arg.Any<IEnumerable<Guid>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns([product]);

        var request = ValidRequest(supplierPublicId, [new CreateOrderItemDto(product.PublicId, 1)]);
        await _sut.Handle(request, CancellationToken.None);

        await _orderRepository.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundFailure()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var request = ValidRequest(Guid.NewGuid(), [new CreateOrderItemDto(Guid.NewGuid(), 1)]);
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_DoesNotAddOrder()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var request = ValidRequest(Guid.NewGuid(), [new CreateOrderItemDto(Guid.NewGuid(), 1)]);
        await _sut.Handle(request, CancellationToken.None);

        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductInItemsNotFound_ReturnsNotFoundFailure()
    {
        var supplierPublicId = Guid.NewGuid();
        _supplierRepository.GetByPublicIdAsync(supplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        // Repository returns empty list — none of the requested product IDs are found
        _productRepository.GetByPublicIdsAsync(
            Arg.Any<IEnumerable<Guid>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var request = ValidRequest(supplierPublicId, [new CreateOrderItemDto(Guid.NewGuid(), 1)]);
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }
}

public sealed class GetOrderByIdRequestHandlerTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetOrderByIdRequestHandler _sut;

    public GetOrderByIdRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetOrderByIdRequestHandler(_orderRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundFailure()
    {
        _orderRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var result = await _sut.Handle(new GetOrderByIdRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }
}

public sealed class GetAllOrdersRequestHandlerTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetAllOrdersRequestHandler _sut;

    public GetAllOrdersRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetAllOrdersRequestHandler(_orderRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ReturnsSuccessWithEmptyItems()
    {
        _orderRepository.GetAllAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>(),
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Items: Enumerable.Empty<Order>(), TotalCount: 0));

        var request = new GetAllOrdersRequest(null, null, true, 1, 10);
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PassesPaginationParametersToRepository()
    {
        _orderRepository.GetAllAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>(),
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Items: Enumerable.Empty<Order>(), TotalCount: 0));

        var request = new GetAllOrdersRequest("term", "date", false, 2, 15);
        await _sut.Handle(request, CancellationToken.None);

        await _orderRepository.Received(1).GetAllAsync(
            "term", "date", false, 2, 15, Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}

public sealed class UpdateOrderRequestHandlerTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly UpdateOrderRequestHandler _sut;

    public UpdateOrderRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new UpdateOrderRequestHandler(
            _orderRepository, _supplierRepository, _unitOfWork, _currentUserAccessor);
    }

    private static UpdateOrderRequest ValidRequest() => new(
        Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), "some notes");

    [Fact]
    public async Task Handle_WhenOrderAndSupplierExist_UpdatesAndReturnsSuccess()
    {
        var request = ValidRequest();
        _orderRepository.GetByPublicIdAsync(request.PublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Notes.Should().Be("some notes");
    }

    [Fact]
    public async Task Handle_WhenOrderAndSupplierExist_SavesChanges()
    {
        var request = ValidRequest();
        _orderRepository.GetByPublicIdAsync(request.PublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        await _sut.Handle(request, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundFailure()
    {
        var request = ValidRequest();
        _orderRepository.GetByPublicIdAsync(request.PublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundFailure()
    {
        var request = ValidRequest();
        _orderRepository.GetByPublicIdAsync(request.PublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderOrSupplierNotFound_DoesNotSave()
    {
        var request = ValidRequest();
        _orderRepository.GetByPublicIdAsync(request.PublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        await _sut.Handle(request, CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class DeleteOrderRequestHandlerTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly DeleteOrderRequestHandler _sut;

    public DeleteOrderRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new DeleteOrderRequestHandler(_orderRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());

        var result = await _sut.Handle(new DeleteOrderRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenOrderExists_CallsDeleteAndSaves()
    {
        var publicId = Guid.NewGuid();
        var order = EntityFactory.CreateOrder();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(order);

        await _sut.Handle(new DeleteOrderRequest(publicId), CancellationToken.None);

        _orderRepository.Received(1).Delete(order);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundFailure()
    {
        _orderRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var result = await _sut.Handle(new DeleteOrderRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_DoesNotDeleteOrSave()
    {
        _orderRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        await _sut.Handle(new DeleteOrderRequest(Guid.NewGuid()), CancellationToken.None);

        _orderRepository.DidNotReceive().Delete(Arg.Any<Order>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
