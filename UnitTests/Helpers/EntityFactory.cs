using Domain.Entities;
using Domain.Enums;

namespace UnitTests.Helpers;

/// <summary>
/// Centralized factory for creating test domain entities.
/// Entities use internal/private constructors so we must go through the public
/// static factory methods exposed on each aggregate.
/// </summary>
internal static class EntityFactory
{
    public static Category CreateCategory(
        string name = "Test Category",
        string? description = "desc",
        Guid? tenantId = null)
        => Category.Create(name, description, tenantId ?? Guid.NewGuid());

    public static Supplier CreateSupplier(
        string name = "Test Supplier",
        Guid? tenantId = null)
        => Supplier.CreateSupplier(name, "Contact", "contact@test.com", "000", tenantId ?? Guid.NewGuid());

    public static Product CreateProduct(
        string name = "Test Product",
        decimal price = 10m,
        int minimumStock = 0,
        int categoryId = 1,
        int supplierId = 1,
        Guid? tenantId = null)
        => Product.CreateProduct(name, null, price, minimumStock, categoryId, supplierId, tenantId ?? Guid.NewGuid());

    public static Order CreateOrder(
        int supplierId = 1,
        Guid? tenantId = null)
        => Order.CreateOrder(supplierId, DateTime.UtcNow.AddDays(-1), null, tenantId ?? Guid.NewGuid());
}
