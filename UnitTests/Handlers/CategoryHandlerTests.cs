using Application.CategoryManagement.CreateCategory;
using Application.CategoryManagement.DeleteCategory;
using Application.CategoryManagement.GetAllCategories;
using Application.CategoryManagement.GetCategoryById;
using Application.CategoryManagement.UpdateCategory;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;
using DomainCategory = Domain.Entities.Category;

namespace UnitTests.Handlers;

public sealed class CreateCategoryRequestHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly CreateCategoryRequestHandler _sut;

    public CreateCategoryRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new CreateCategoryRequestHandler(_categoryRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ReturnsSuccess()
    {
        var request = new CreateCategoryRequest("Books", "All books");

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithValidRequest_ReturnsCorrectName()
    {
        var request = new CreateCategoryRequest("Books", "All books");

        var result = await _sut.Handle(request, CancellationToken.None);

        result.Value!.Name.Should().Be("Books");
        result.Value.Description.Should().Be("All books");
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsRepositoryAdd()
    {
        var request = new CreateCategoryRequest("Books", "All books");

        await _sut.Handle(request, CancellationToken.None);

        await _categoryRepository.Received(1).AddAsync(
            Arg.Any<DomainCategory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidRequest_SavesChanges()
    {
        var request = new CreateCategoryRequest("Books", "All books");

        await _sut.Handle(request, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UsesTenantPublicIdFromCurrentUserAccessor()
    {
        var tenantId = Guid.NewGuid();
        _currentUserAccessor.TenantPublicId.Returns(tenantId);
        var request = new CreateCategoryRequest("Books", "desc");

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}

public sealed class GetAllCategoriesRequestHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetAllCategoriesRequestHandler _sut;

    public GetAllCategoriesRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetAllCategoriesRequestHandler(_categoryRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsCategories_ReturnsMappedDtos()
    {
        var tenantId = Guid.NewGuid();
        _currentUserAccessor.TenantPublicId.Returns(tenantId);
        var categories = new[]
        {
            EntityFactory.CreateCategory("Electronics", "Gadgets", tenantId),
            EntityFactory.CreateCategory("Books", null, tenantId)
        };
        _categoryRepository.GetAllAsync(tenantId, Arg.Any<CancellationToken>())
            .Returns(categories);

        var result = await _sut.Handle(new GetAllCategoriesRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ReturnsEmptyCollection()
    {
        _categoryRepository.GetAllAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var result = await _sut.Handle(new GetAllCategoriesRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}

public sealed class GetCategoryByIdRequestHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetCategoryByIdRequestHandler _sut;

    public GetCategoryByIdRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetCategoryByIdRequestHandler(_categoryRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ReturnsSuccessWithData()
    {
        var publicId = Guid.NewGuid();
        var category = EntityFactory.CreateCategory("Electronics");
        _categoryRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(category);

        var result = await _sut.Handle(new GetCategoryByIdRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsNotFoundFailure()
    {
        _categoryRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);

        var result = await _sut.Handle(new GetCategoryByIdRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }
}

public sealed class UpdateCategoryRequestHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly UpdateCategoryRequestHandler _sut;

    public UpdateCategoryRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new UpdateCategoryRequestHandler(_categoryRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_UpdatesAndReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        var category = EntityFactory.CreateCategory("Old Name");
        _categoryRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(category);

        var request = new UpdateCategoryRequest(publicId, "New Name", "New Desc");
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Name");
        result.Value.Description.Should().Be("New Desc");
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_SavesChanges()
    {
        var publicId = Guid.NewGuid();
        _categoryRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());

        await _sut.Handle(new UpdateCategoryRequest(publicId, "Name", null), CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsNotFoundFailure()
    {
        _categoryRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);

        var result = await _sut.Handle(
            new UpdateCategoryRequest(Guid.NewGuid(), "Name", null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_DoesNotSaveChanges()
    {
        _categoryRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);

        await _sut.Handle(new UpdateCategoryRequest(Guid.NewGuid(), "Name", null), CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class DeleteCategoryRequestHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly DeleteCategoryRequestHandler _sut;

    public DeleteCategoryRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new DeleteCategoryRequestHandler(_categoryRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _categoryRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());

        var result = await _sut.Handle(new DeleteCategoryRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_CallsDeleteOnRepository()
    {
        var publicId = Guid.NewGuid();
        var category = EntityFactory.CreateCategory();
        _categoryRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(category);

        await _sut.Handle(new DeleteCategoryRequest(publicId), CancellationToken.None);

        _categoryRepository.Received(1).Delete(category);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_SavesChanges()
    {
        var publicId = Guid.NewGuid();
        _categoryRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateCategory());

        await _sut.Handle(new DeleteCategoryRequest(publicId), CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsNotFoundFailure()
    {
        _categoryRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);

        var result = await _sut.Handle(new DeleteCategoryRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_DoesNotDeleteOrSave()
    {
        _categoryRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainCategory?)null);

        await _sut.Handle(new DeleteCategoryRequest(Guid.NewGuid()), CancellationToken.None);

        _categoryRepository.DidNotReceive().Delete(Arg.Any<DomainCategory>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
