using Domain.Enums;

namespace Domain.Entities;

public class Order : AuditableEntity
{
    public int ProductId { get; private set; }
    public int SupplierId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public OrderStatus OrderStatus { get; private set; }
    public Supplier? Supplier { get; private set; }
    public Product? Product { get; private set; }



}
