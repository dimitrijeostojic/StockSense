namespace Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int MinimumStockQuantity { get; private set; }
    public int CategoryId { get; private set; }
    public int SupplierId { get; private set; }
    public Category? Category { get; private set; }
    public Supplier? Supplier { get; private set; }
    public ICollection<StockEntry> StockEntries { get; set; } = [];

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
}
