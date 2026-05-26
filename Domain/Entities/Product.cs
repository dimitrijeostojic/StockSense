using Domain.Enums;

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int MinimumStockQuantity { get; private set; }
    public int CategoryId { get; private set; }
    public int SupplierId { get; private set; }
    public Category? Category { get; private set; }
    public Supplier? Supplier { get; private set; }
    private readonly List<StockEntry> _stockEntries = [];
    public IReadOnlyCollection<StockEntry> StockEntries => _stockEntries.AsReadOnly();
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public static Product CreateProduct(string name, string description, decimal price, int minimumStockQuantity, int categoryId, int supplierId)
    {
        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            MinimumStockQuantity = minimumStockQuantity,
            CategoryId = categoryId,
            SupplierId = supplierId
        };
    }

    public void AddStockEntry(int quantity, StockEntryType type, string notes)
    {
        var entry = StockEntry.Create(Id, quantity, DateTime.UtcNow, notes, type);
        _stockEntries.Add(entry);
    }
}
