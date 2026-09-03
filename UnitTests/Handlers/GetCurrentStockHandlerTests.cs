using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.ProductManagement.GetCurrentStock;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;
using DomainProduct = Domain.Entities.Product;

namespace UnitTests.Handlers;

public sealed class GetCurrentStockRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetCurrentStockRequestHandler _sut;

    public GetCurrentStockRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetCurrentStockRequestHandler(_productRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        var result = await _sut.Handle(new GetCurrentStockRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductHasNoStockEntries_ReturnsZero()
    {
        var publicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        _productRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(new GetCurrentStockRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.StockNumber.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenProductHasInAndOutEntries_ReturnsNetStock()
    {
        var publicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        product.AddStockEntry(10, StockEntryType.In, null);
        product.AddStockEntry(4, StockEntryType.Out, null);

        _productRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(new GetCurrentStockRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.StockNumber.Should().Be(6);
    }

    [Fact]
    public async Task Handle_WhenProductHasOnlyInEntries_ReturnsTotalInQuantity()
    {
        var publicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct(minimumStock: 0);
        product.AddStockEntry(15, StockEntryType.In, null);
        product.AddStockEntry(5, StockEntryType.In, null);

        _productRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _sut.Handle(new GetCurrentStockRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.StockNumber.Should().Be(20);
    }
}
