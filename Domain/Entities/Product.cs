using Domain.Enums;
using Domain.Events;
using Domain.Primitives;

namespace Domain.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public UnitOfMeasurement UnitOfMeasure { get; private set; }
    public int MinimumStockQuantity { get; private set; }
    public int CategoryId { get; private set; }
    public int SupplierId { get; private set; }
    public Category? Category { get; private set; }
    public Supplier? Supplier { get; private set; }
    private readonly List<StockEntry> _stockEntries = [];
    public IReadOnlyCollection<StockEntry> StockEntries => _stockEntries;
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public Guid TenantPublicId { get; private set; }

    private Product(string name, string sku, string? description, decimal price, int minimumStockQuantity, UnitOfMeasurement unitOfMeasure, int categoryId, int supplierId, Guid tenantPublicId)
    {
        Name = name;
        Sku = sku;
        Description = description;
        Price = price;
        MinimumStockQuantity = minimumStockQuantity;
        UnitOfMeasure = unitOfMeasure;
        CategoryId = categoryId;
        SupplierId = supplierId;
        TenantPublicId = tenantPublicId;
    }

    public static Product CreateProduct(string name, string sku, string? description, decimal price, int minimumStockQuantity, UnitOfMeasurement unitOfMeasure, int categoryId, int supplierId, Guid tenantPublicId)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(sku);
        return new Product(name, sku, description, price, minimumStockQuantity, unitOfMeasure, categoryId, supplierId, tenantPublicId);
    }

    public StockEntry AddStockEntry(int quantity, StockEntryType type, string? notes)
    {
        var entry = StockEntry.Create(quantity, DateTime.UtcNow, notes, type);
        _stockEntries.Add(entry);
        var currentStock = StockEntries.Sum(se => se.StockEntryType == StockEntryType.In ? se.Quantity : -se.Quantity);
        if (currentStock < MinimumStockQuantity)
        {
            RaiseDomainEvent(new LowStockDomainEvent(PublicId, TenantPublicId, currentStock));
        }
        return entry;
    }

    public Product WithName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
        return this;
    }
    public Product WithSku(string sku)
    {
        ArgumentException.ThrowIfNullOrEmpty(sku);
        Sku = sku;
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
    public Product WithUnitOfMeasurement(UnitOfMeasurement unitOfMeasure)
    {
        UnitOfMeasure = unitOfMeasure;
        return this;
    }

}
