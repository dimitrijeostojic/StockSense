using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.OrderManagement.UpdateOrderStatus;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryInterfaces;
using FluentAssertions;
using NSubstitute;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Handlers;

public sealed class UpdateOrderStatusRequestHandlerTests
{
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserAccessor _currentUserAccessor = Substitute.For<ICurrentUserAccessor>();

    private readonly UpdateOrderStatusRequestHandler _sut;

    public UpdateOrderStatusRequestHandlerTests()
    {
        _currentUserAccessor.TenantPublicId.Returns(Guid.NewGuid());
        _sut = new UpdateOrderStatusRequestHandler(_orderRepository, _currentUserAccessor, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundFailure()
    {
        _orderRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var result = await _sut.Handle(
            new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.Confirmed), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenValidTransition_PendingToConfirmed_ReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());

        var result = await _sut.Handle(
            new UpdateOrderStatusRequest(publicId, OrderStatus.Confirmed), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NewStatus.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_WhenValidTransition_PendingToCancelled_ReturnsSuccess()
    {
        var publicId = Guid.NewGuid();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());

        var result = await _sut.Handle(
            new UpdateOrderStatusRequest(publicId, OrderStatus.Cancelled), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NewStatus.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_WhenInvalidTransition_PendingToReceived_ReturnsInvalidTransitionFailure()
    {
        var publicId = Guid.NewGuid();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());

        var result = await _sut.Handle(
            new UpdateOrderStatusRequest(publicId, OrderStatus.Received), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ApplicationErrors.InvalidOrderStatusTransition);
    }

    [Fact]
    public async Task Handle_WhenValidTransition_SavesChanges()
    {
        var publicId = Guid.NewGuid();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());

        await _sut.Handle(
            new UpdateOrderStatusRequest(publicId, OrderStatus.Confirmed), CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_DoesNotSave()
    {
        _orderRepository.GetByPublicIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        await _sut.Handle(
            new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.Confirmed), CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInvalidTransition_DoesNotSave()
    {
        var publicId = Guid.NewGuid();
        _orderRepository.GetByPublicIdAsync(publicId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(EntityFactory.CreateOrder());

        await _sut.Handle(
            new UpdateOrderStatusRequest(publicId, OrderStatus.Received), CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
