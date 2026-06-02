using Domain.Enums;

namespace Domain.Entities;

public class Order : AuditableEntity
{
    public int SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string? Notes { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public Supplier? Supplier { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private readonly List<OrderItem> _orderItems = [];

    public static Order CreateOrder(int supplierId, DateTime orderDate, string? notes)
    {
        return new Order
        {
            SupplierId = supplierId,
            OrderDate = orderDate,
            Notes = notes,
            OrderStatus = OrderStatus.Pending
        };
    }

    public void AddItem(int productId, int quantity, decimal unitPrice)
    {
        var item = OrderItem.Create(productId, quantity, unitPrice);
        _orderItems.Add(item);
    }
    public void RemoveItem(Guid OrderItemPublicId)
    {
        var item = _orderItems.FirstOrDefault(oi => oi.PublicId == OrderItemPublicId);
        if (item == null)
        {
            throw new Exception($"Order item with PublicId {OrderItemPublicId} not found.");
        }
        _orderItems.Remove(item);
    }

    public Order WithSupplierId(int supplierId)
    {
        SupplierId = supplierId;
        return this;
    }
    public Order WithOrderDate(DateTime orderDate)
    {
        OrderDate = orderDate;
        return this;
    }
    public Order WithNotes(string? notes)
    {
        Notes = notes;
        return this;
    }
    public Order WithOrderStatus(OrderStatus orderStatus)
    {
        OrderStatus = orderStatus;
        return this;
    }


}
