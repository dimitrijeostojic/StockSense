namespace Domain.Entities;

public class OrderItem : Entity
{
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Product? Product { get; private set; }
    public Order? Order { get; private set; }

    private OrderItem()
    {

    }


    internal static OrderItem Create(int productId, int quantity, decimal unitPrice)
    {
        return new OrderItem
        {
            Quantity = quantity,
            UnitPrice = unitPrice,
            ProductId = productId
        };
    }
}
