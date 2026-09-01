using Domain.Entities;
using Domain.Enums;
using System.Reflection;

namespace UnitTests.Helpers;

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

    public static Product CreateProductWithNavigation(
        string name = "Test Product",
        decimal price = 10m,
        int minimumStock = 0,
        Guid? tenantId = null)
    {
        var product = CreateProduct(name, price, minimumStock, tenantId: tenantId);
        SetPrivate(product, "Category", CreateCategory());
        SetPrivate(product, "Supplier", CreateSupplier());
        return product;
    }

    public static Order CreateOrder(
        int supplierId = 1,
        Guid? tenantId = null)
        => Order.CreateOrder(supplierId, DateTime.UtcNow.AddDays(-1), null, tenantId ?? Guid.NewGuid());

    public static Order CreateOrderWithNavigation(Guid? tenantId = null)
    {
        var order = CreateOrder(tenantId: tenantId);
        SetPrivate(order, "Supplier", CreateSupplier());
        return order;
    }

    internal static void SetPrivate(object obj, string propertyName, object? value)
    {
        var prop = obj.GetType().GetProperty(propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                $"Property '{propertyName}' not found on '{obj.GetType().Name}'.");
        prop.SetValue(obj, value);
    }

    internal static T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                $"Field '{fieldName}' not found on '{obj.GetType().Name}'.");
        return (T)field.GetValue(obj)!;
    }
}
