using Domain.Enums;

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; private set; } = null!;
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

    private Product()
    {

    }

    public static Product CreateProduct(string name, string? description, decimal price, int minimumStockQuantity, int categoryId, int supplierId)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
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
        var entry = StockEntry.Create(quantity, DateTime.UtcNow, notes, type);
        _stockEntries.Add(entry);
    }

    public Product WithName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
        return this;
    }
    public Product WithDescription(string? description)
    {
        Description = description;
        return this;
    }
    public Product WithPrice(decimal price)
    {
        Price = price;
        return this;
    }
    public Product WithMinimumStockQuantity(int minimumStockQuantity)
    {
        MinimumStockQuantity = minimumStockQuantity;
        return this;
    }
    public Product WithCategoryId(int categoryId)
    {
        CategoryId = categoryId;
        return this;
    }
    public Product WithSupplierId(int supplierId)
    {
        SupplierId = supplierId;
        return this;
    }

}
