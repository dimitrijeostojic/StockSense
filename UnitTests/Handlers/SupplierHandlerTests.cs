using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.SupplierManagement.CreateSupplier;
using Application.SupplierManagement.DeleteSupplier;
using Application.SupplierManagement.GetAllSuppliers;
using Application.SupplierManagement.GetSupplierById;
using Application.SupplierManagement.UpdateSupplier;
using Domain.Abstractions;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class CreateSupplierRequestHandlerTests
{
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly CreateSupplierRequestHandler _sut;

    public CreateSupplierRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new CreateSupplierRequestHandler(_supplierRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ReturnsSuccess()
    {
        var request = new CreateSupplierRequest("Acme", "John", "j@j.com", "000");

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Acme");
        result.Value.ContactName.Should().Be("John");
    }

    [Fact]
    public async Task Handle_WithValidRequest_AddsSupplierAndSaves()
    {
        var request = new CreateSupplierRequest("Acme", "John", null, "000");

        await _sut.Handle(request, CancellationToken.None);

        await _supplierRepository.Received(1).AddAsync(
            Arg.Any<DomainSupplier>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNullOptionalFields_ReturnsSuccessWithNullFields()
    {
        var request = new CreateSupplierRequest("Acme", null!, null, null!);

        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ContactName.Should().BeNull();
        result.Value.ContactEmail.Should().BeNull();
        result.Value.ContactPhone.Should().BeNull();
    }
}

public sealed class GetSupplierByIdRequestHandlerTests
{
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetSupplierByIdRequestHandler _sut;

    public GetSupplierByIdRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetSupplierByIdRequestHandler(_supplierRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_ReturnsSuccessWithData()
    {
        var publicId = Guid.NewGuid();
        var supplier = EntityFactory.CreateSupplier("Acme");
        _supplierRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(supplier);

        var result = await _sut.Handle(new GetSupplierByIdRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Acme");
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundFailure()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var result = await _sut.Handle(new GetSupplierByIdRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }
}

public sealed class GetAllSuppliersRequestHandlerTests
{
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly GetAllSuppliersRequestHandler _sut;

    public GetAllSuppliersRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new GetAllSuppliersRequestHandler(_supplierRepository, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ReturnsSuccessWithEmptyCollection()
    {
        _supplierRepository.GetAllAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>(),
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Items: Enumerable.Empty<DomainSupplier>(), TotalCount: 0));

        var request = new GetAllSuppliersRequest(null, null, true, 1, 10);
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PassesPaginationParametersToRepository()
    {
        _supplierRepository.GetAllAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>(),
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Items: Enumerable.Empty<DomainSupplier>(), TotalCount: 0));

        var request = new GetAllSuppliersRequest("search", "name", false, 3, 20);
        await _sut.Handle(request, CancellationToken.None);

        await _supplierRepository.Received(1).GetAllAsync(
            "search", "name", false, 3, 20, Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}

public sealed class UpdateSupplierRequestHandlerTests
{
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly UpdateSupplierRequestHandler _sut;

    public UpdateSupplierRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new UpdateSupplierRequestHandler(_supplierRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_UpdatesAndReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _supplierRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier("Old Name"));

        var request = new UpdateSupplierRequest(publicId, "New Name", "Alice", "a@a.com", "111");
        var result = await _sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Name");
        result.Value.ContactName.Should().Be("Alice");
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_SavesChanges()
    {
        var publicId = Guid.NewGuid();
        _supplierRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        await _sut.Handle(new UpdateSupplierRequest(publicId, "Name", null, null, null), CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundFailure()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var result = await _sut.Handle(
            new UpdateSupplierRequest(Guid.NewGuid(), "Name", null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_DoesNotSave()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        await _sut.Handle(
            new UpdateSupplierRequest(Guid.NewGuid(), "Name", null, null, null), CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public sealed class DeleteSupplierRequestHandlerTests
{
    private readonly ISupplierRepository _supplierRepository = Substitute.For<ISupplierRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly DeleteSupplierRequestHandler _sut;

    public DeleteSupplierRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new DeleteSupplierRequestHandler(_supplierRepository, _unitOfWork, _currentUserAccessor);
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_ReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _supplierRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateSupplier());

        var result = await _sut.Handle(new DeleteSupplierRequest(publicId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_CallsDeleteAndSaves()
    {
        var publicId = Guid.NewGuid();
        var supplier = EntityFactory.CreateSupplier();
        _supplierRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(supplier);

        await _sut.Handle(new DeleteSupplierRequest(publicId), CancellationToken.None);

        _supplierRepository.Received(1).Delete(supplier);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundFailure()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        var result = await _sut.Handle(new DeleteSupplierRequest(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_DoesNotDeleteOrSave()
    {
        _supplierRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((DomainSupplier?)null);

        await _sut.Handle(new DeleteSupplierRequest(Guid.NewGuid()), CancellationToken.None);

        _supplierRepository.DidNotReceive().Delete(Arg.Any<DomainSupplier>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
