using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.ProductManagement.CreateProduct;
using Application.ProductManagement.DeleteProduct;
using Application.ProductManagement.GetAllProducts;
using Application.ProductManagement.GetProductById;
using Application.ProductManagement.UpdateProduct;
using Domain.Abstractions;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class CreateProductRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly CreateProductRequestHandler _sut;

    public CreateProductRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new CreateProductRequestHandler(
            _productRepository, _categoryRepository, _supplierRepository,
            _unitOfWork, _currentUserAccessor);
    }

    private static CreateProductRequest ValidRequest() => new(
        "Widget", "A widget", 9.99m, 5, Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public async Task Handle_WhenCategoryAndSupplierExist_ReturnsSuccess()
    {
        var request = ValidRequest();
        var category = EntityFactory.CreateCategory();
        var supplier = EntityFactory.CreateSupplier();
        _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(category);
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(supplier);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Widget");
        result.Value.Price.Should().Be(9.99m);
    }

    [Fact]
    public async Task Handle_WhenCategoryAndSupplierExist_AddsProductAndSaves()
    {
        var request = ValidRequest();
        _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        await _sut.Handle(request, CancellationToken.None);

        await _productRepository.Received(1).AddAsync(
            Arg.Any<DomainProduct>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsNotFoundFailure()
    {
        var request = ValidRequest();
        _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);
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
        _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCategoryOrSupplierNotFound_DoesNotAddProduct()
    {
        var request = ValidRequest();
        _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);
        _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        await _sut.Handle(request, CancellationToken.None);

        await _productRepository.DidNotReceive().AddAsync(
            Arg.Any<DomainProduct>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class GetProductByIdRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetProductByIdRequestHandler _sut;

    public GetProductByIdRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetProductByIdRequestHandler(_productRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        var result = await _sut.Handle(new GetProductByIdRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }
}

public sealed class GetAllProductsRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetAllProductsRequestHandler _sut;

    public GetAllProductsRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetAllProductsRequestHandler(_productRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmptyList_ReturnsSuccessWithEmptyItems()
    {
        _productRepository.GetAllAsync(
            Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((Items: Enumerable.Empty<DomainProduct>(), TotalCount: 0));

        var request = new GetAllProductsRequest(null, null, true, 1, 10);
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PassesPaginationParametersToRepository()
    {
        _productRepository.GetAllAsync(
            Arg.Any<Guid>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((Items: Enumerable.Empty<DomainProduct>(), TotalCount: 0));

        var request = new GetAllProductsRequest("search", "name", false, 2, 25);
        await _sut.Handle(request, CancellationToken.None);

        await _productRepository.Received(1).GetAllAsync(
            Arg.Any<Guid>(), "search", "name", false, 2, 25, Arg.Any<CancellationToken>());
    }
}

public sealed class UpdateProductRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly UpdateProductRequestHandler _sut;

    public UpdateProductRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new UpdateProductRequestHandler(
            _productRepository, _supplierRepository, _categoryRepository,
            _unitOfWork, _currentUserAccessor);
    }

    private static UpdateProductRequest ValidRequest() => new(
        Guid.NewGuid(), "Updated Widget", "desc", 19.99m, 10,
        Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public async Task Handle_WhenAllEntitiesFound_UpdatesAndReturnsSuccess()
    {
        var request = ValidRequest();
        var product = EntityFactory.CreateProduct();
        var category = EntityFactory.CreateCategory();
        var supplier = EntityFactory.CreateSupplier();

        _productRepository.GetByPublicIdAsync(request.ProductPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);
        _categoryRepository.GetByPublicIdAsync(request.CategoryId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(category);
        _supplierRepository.GetByPublicIdAsync(request.SupplierId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(supplier);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Updated Widget");
        result.Value.Price.Should().Be(19.99m);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        var request = ValidRequest();
        _productRepository.GetByPublicIdAsync(request.ProductPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);
        _categoryRepository.GetByPublicIdAsync(request.CategoryId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());
        _supplierRepository.GetByPublicIdAsync(request.SupplierId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsNotFoundFailure()
    {
        var request = ValidRequest();
        _productRepository.GetByPublicIdAsync(request.ProductPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateProduct());
        _categoryRepository.GetByPublicIdAsync(request.CategoryId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);
        _supplierRepository.GetByPublicIdAsync(request.SupplierId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundFailure()
    {
        var request = ValidRequest();
        _productRepository.GetByPublicIdAsync(request.ProductPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateProduct());
        _categoryRepository.GetByPublicIdAsync(request.CategoryId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());
        _supplierRepository.GetByPublicIdAsync(request.SupplierId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenAllEntitiesFound_SavesChanges()
    {
        var request = ValidRequest();
        _productRepository.GetByPublicIdAsync(request.ProductPublicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateProduct());
        _categoryRepository.GetByPublicIdAsync(request.CategoryId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());
        _supplierRepository.GetByPublicIdAsync(request.SupplierId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        await _sut.Handle(request, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class DeleteProductRequestHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly DeleteProductRequestHandler _sut;

    public DeleteProductRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new DeleteProductRequestHandler(_productRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _productRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateProduct());

        var result = await _sut.Handle(new DeleteProductRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenProductExists_CallsDeleteAndSaves()
    {
        var publicId = Guid.NewGuid();
        var product = EntityFactory.CreateProduct();
        _productRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        await _sut.Handle(new DeleteProductRequest(publicId), CancellationToken.None);

        _productRepository.Received(1).Delete(product);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundFailure()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        var result = await _sut.Handle(new DeleteProductRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_DoesNotDeleteOrSave()
    {
        _productRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainProduct?)null);

        await _sut.Handle(new DeleteProductRequest(Guid.NewGuid()), CancellationToken.None);

        _productRepository.DidNotReceive().Delete(Arg.Any<DomainProduct>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
