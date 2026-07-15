using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.ProductManagement.CreateStockEntry;
using Application.ProductManagement.GetAllStockEntries;
using Application.ProductManagement.GetStockEntryByProductId;
using Domain.Abstractions;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using MediatR;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class CreateStockEntryRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    private readonly CreateStockEntryRequestHandler _sut;

    public CreateStockEntryRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new CreateStockEntryRequestHandler(
            _productRepository, _unitOfWork, _currentUserAccessor, _mediator);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ReturnsSuccessWithEntry()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var request = new CreateStockEntryRequest(productPublicId, 10, "initial stock", StockEntryType.In);
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Quantity.Should().Be(10);
        result.Value.StockEntryType.Should().Be(StockEntryType.In);
        result.Value.Notes.Should().Be("initial stock");
    }

    [Fact]
    public async Task Handle_WhenProductExists_SavesChanges()
    {
        var productPublicId = Guid.NewGuid();
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateProduct(minimumStock: 0));

        await _sut.Handle(
            new CreateStockEntryRequest(productPublicId, 5, null, StockEntryType.In),
            CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        var result = await _sut.Handle(
            new CreateStockEntryRequest(Guid.NewGuid(), 10, null, StockEntryType.In),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_DoesNotSave()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        await _sut.Handle(
            new CreateStockEntryRequest(Guid.NewGuid(), 10, null, StockEntryType.In),
            CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenLowStockDomainEventRaised_PublishesEventViaMediator()
    {
        var productPublicId = Guid.NewGuid();
        // minStock = 100, adding only 1 → triggers LowStockDomainEvent
        var product = EntityFactory.CreateProduct(minimumStock: 100);
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(
            new CreateStockEntryRequest(productPublicId, 1, null, StockEntryType.In),
            CancellationToken.None);

        await _mediator.Received(1).Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoLowStockEvent_DoesNotPublish()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(
            new CreateStockEntryRequest(productPublicId, 50, null, StockEntryType.In),
            CancellationToken.None);

        await _mediator.DidNotReceive().Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AfterPublishingEvents_ClearsDomainEvents()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 100);
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(
            new CreateStockEntryRequest(productPublicId, 1, null, StockEntryType.In),
            CancellationToken.None);

        product.DomainEvents.Should().BeEmpty();
    }
}

public sealed class GetAllStockEntriesRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetAllStockEntriesRequestHandler _sut;

    public GetAllStockEntriesRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetAllStockEntriesRequestHandler(_productRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        var result = await _sut.Handle(new GetAllStockEntriesRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductFoundWithNoEntries_ReturnsEmptyCollection()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct();
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(new GetAllStockEntriesRequest(productPublicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenProductFoundWithEntries_ReturnsMappedDtos()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        product.AddStockEntry(5, StockEntryType.In, "batch 1");
        product.AddStockEntry(3, StockEntryType.Out, null);
        product.ClearDomainEvents();

        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(new GetAllStockEntriesRequest(productPublicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }
}

public sealed class GetStockEntryByIdRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetStockEntryByIdRequestHandler _sut;

    public GetStockEntryByIdRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetStockEntryByIdRequestHandler(_productRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        var result = await _sut.Handle(
            new GetStockEntryByIdRequest(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductFoundButEntryNotFound_ReturnsNotFoundFailure()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(
            new GetStockEntryByIdRequest(productPublicId, Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductAndEntryFound_ReturnsSuccessWithData()
    {
        var productPublicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        var entry = product.AddStockEntry(7, StockEntryType.In, "notes");
        product.ClearDomainEvents();

        _productRepository.GetByPublicIdAsync(productPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(
            new GetStockEntryByIdRequest(productPublicId, entry.PublicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Quantity.Should().Be(7);
        result.Value.StockEntryType.Should().Be(StockEntryType.In);
        result.Value.Notes.Should().Be("notes");
    }
}
