using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class OrderEntityTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly DateTime ValidDate = DateTime.UtcNow.AddDays(-1);

    private static Order BuildPendingOrder()
        => Order.CreateOrder(1, ValidDate, null, TenantId);

    [Fact]
    public void CreateOrder_SetsStatusToPending()
    {
        var order = BuildPendingOrder();

        order.OrderStatus.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void CreateOrder_SetsCorrectSupplierId()
    {
        var order = Order.CreateOrder(42, ValidDate, "note", TenantId);

        order.SupplierId.Should().Be(42);
        order.Notes.Should().Be("note");
        order.TenantPublicId.Should().Be(TenantId);
    }

    [Fact]
    public void CreateOrder_HasEmptyOrderItems()
    {
        var order = BuildPendingOrder();

        order.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_AddsOrderItem()
    {
        var order = BuildPendingOrder();

        order.AddItem(productId: 1, quantity: 3, unitPrice: 10m);

        order.OrderItems.Should().HaveCount(1);
        order.OrderItems.First().Quantity.Should().Be(3);
        order.OrderItems.First().UnitPrice.Should().Be(10m);
    }

    [Fact]
    public void AddItem_MultipleItems_AllPresent()
    {
        var order = BuildPendingOrder();

        order.AddItem(1, 2, 5m);
        order.AddItem(2, 4, 8m);

        order.OrderItems.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveItem_ExistingItem_RemovesIt()
    {
        var order = BuildPendingOrder();
        order.AddItem(1, 3, 10m);
        var item = order.OrderItems.First();

        order.RemoveItem(item.PublicId);

        order.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_NonExistentPublicId_ThrowsException()
    {
        var order = BuildPendingOrder();

        var act = () => order.RemoveItem(Guid.NewGuid());

        act.Should().Throw<Exception>();
    }

    // --- WithOrderStatus transitions ---

    [Fact]
    public void WithOrderStatus_PendingToConfirmed_Succeeds()
    {
        var order = BuildPendingOrder();

        var result = order.WithOrderStatus(OrderStatus.Confirmed);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void WithOrderStatus_PendingToCancelled_Succeeds()
    {
        var order = BuildPendingOrder();

        var result = order.WithOrderStatus(OrderStatus.Cancelled);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void WithOrderStatus_ConfirmedToReceived_Succeeds()
    {
        var order = BuildPendingOrder();
        order.WithOrderStatus(OrderStatus.Confirmed);

        var result = order.WithOrderStatus(OrderStatus.Received);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Received);
    }

    [Fact]
    public void WithOrderStatus_ConfirmedToCancelled_Succeeds()
    {
        var order = BuildPendingOrder();
        order.WithOrderStatus(OrderStatus.Confirmed);

        var result = order.WithOrderStatus(OrderStatus.Cancelled);

        result.IsSuccess.Should().BeTrue();
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void WithOrderStatus_PendingToReceived_ReturnsFailure()
    {
        var order = BuildPendingOrder();

        var result = order.WithOrderStatus(OrderStatus.Received);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Order");
    }

    [Fact]
    public void WithOrderStatus_CancelledToConfirmed_ReturnsFailure()
    {
        var order = BuildPendingOrder();
        order.WithOrderStatus(OrderStatus.Cancelled);

        var result = order.WithOrderStatus(OrderStatus.Confirmed);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void WithOrderStatus_ReceivedToAnything_ReturnsFailure()
    {
        var order = BuildPendingOrder();
        order.WithOrderStatus(OrderStatus.Confirmed);
        order.WithOrderStatus(OrderStatus.Received);
        order.ClearDomainEvents();

        var result = order.WithOrderStatus(OrderStatus.Cancelled);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void WithOrderStatus_ConfirmedToReceived_RaisesDomainEvent()
    {
        var order = BuildPendingOrder();
        order.WithOrderStatus(OrderStatus.Confirmed);

        order.WithOrderStatus(OrderStatus.Received);

        order.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void WithSupplierId_UpdatesSupplierId()
    {
        var order = BuildPendingOrder();

        order.WithSupplierId(99);

        order.SupplierId.Should().Be(99);
    }

    [Fact]
    public void WithNotes_UpdatesNotes()
    {
        var order = BuildPendingOrder();

        order.WithNotes("updated notes");

        order.Notes.Should().Be("updated notes");
    }

    [Fact]
    public void WithOrderDate_UpdatesOrderDate()
    {
        var order = BuildPendingOrder();
        var newDate = DateTime.UtcNow.AddDays(-2);

        order.WithOrderDate(newDate);

        order.OrderDate.Should().Be(newDate);
    }
}
