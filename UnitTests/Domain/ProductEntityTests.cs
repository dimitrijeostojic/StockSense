using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace UnitTests.Domain;

public sealed class ProductEntityTests
{
    private static readonly Guid TenantId = Guid.NewGuid();

    private static Product BuildProduct(
        string name = "Widget",
        decimal price = 9.99m,
        int minStock = 5,
        int categoryId = 1,
        int supplierId = 1)
        => Product.CreateProduct(name, null, price, minStock, categoryId, supplierId, TenantId);

    [Fact]
    public void CreateProduct_WithValidInputs_ReturnsProductWithExpectedValues()
    {
        var product = BuildProduct("Widget", 9.99m, 5, 1, 2);

        product.Name.Should().Be("Widget");
        product.Price.Should().Be(9.99m);
        product.MinimumStockQuantity.Should().Be(5);
        product.CategoryId.Should().Be(1);
        product.SupplierId.Should().Be(2);
        product.TenantPublicId.Should().Be(TenantId);
    }

    [Fact]
    public void CreateProduct_AssignsNewPublicId()
    {
        var product = BuildProduct();

        product.PublicId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void CreateProduct_WithNullName_ThrowsArgumentException()
    {
        var act = () => Product.CreateProduct(null!, null, 1m, 0, 1, 1, TenantId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateProduct_WithEmptyName_ThrowsArgumentException()
    {
        var act = () => Product.CreateProduct(string.Empty, null, 1m, 0, 1, 1, TenantId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateProduct_HasEmptyStockEntries()
    {
        var product = BuildProduct();

        product.StockEntries.Should().BeEmpty();
    }

    [Fact]
    public void AddStockEntry_AddsEntryToCollection()
    {
        var product = BuildProduct(minStock: 0);

        product.AddStockEntry(10, StockEntryType.In, null);

        product.StockEntries.Should().HaveCount(1);
    }

    [Fact]
    public void AddStockEntry_ReturnsEntryWithCorrectQuantityAndType()
    {
        var product = BuildProduct(minStock: 0);

        var entry = product.AddStockEntry(10, StockEntryType.In, "Initial stock");

        entry.Quantity.Should().Be(10);
        entry.StockEntryType.Should().Be(StockEntryType.In);
        entry.Notes.Should().Be("Initial stock");
    }

    [Fact]
    public void AddStockEntry_WhenCurrentStockBelowMinimum_RaisesDomainEvent()
    {
        // MinimumStockQuantity = 100; adding only 5 In → stock(5) < min(100)
        var product = BuildProduct(minStock: 100);

        product.AddStockEntry(5, StockEntryType.In, null);

        product.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void AddStockEntry_WhenCurrentStockAboveOrEqualMinimum_DoesNotRaiseDomainEvent()
    {
        var product = BuildProduct(minStock: 5);

        product.AddStockEntry(10, StockEntryType.In, null);

        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddStockEntry_OutType_DeductsFromCurrentStock()
    {
        var product = BuildProduct(minStock: 0);
        product.AddStockEntry(20, StockEntryType.In, null);
        product.ClearDomainEvents();

        // Taking 18 out → remaining 2, still >= 0 (minStock)
        product.AddStockEntry(18, StockEntryType.Out, null);

        product.StockEntries.Should().HaveCount(2);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var product = BuildProduct(minStock: 100);
        product.AddStockEntry(1, StockEntryType.In, null);

        product.ClearDomainEvents();

        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void WithName_UpdatesName()
    {
        var product = BuildProduct("Old");

        product.WithName("New");

        product.Name.Should().Be("New");
    }

    [Fact]
    public void WithName_WithNullOrEmpty_ThrowsArgumentException()
    {
        var product = BuildProduct();

        var act = () => product.WithName(string.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithPrice_UpdatesPrice()
    {
        var product = BuildProduct(price: 1m);

        product.WithPrice(99.99m);

        product.Price.Should().Be(99.99m);
    }

    [Fact]
    public void WithMinimumStockQuantity_UpdatesMinimumStockQuantity()
    {
        var product = BuildProduct(minStock: 1);

        product.WithMinimumStockQuantity(50);

        product.MinimumStockQuantity.Should().Be(50);
    }

    [Fact]
    public void WithCategoryId_UpdatesCategoryId()
    {
        var product = BuildProduct(categoryId: 1);

        product.WithCategoryId(99);

        product.CategoryId.Should().Be(99);
    }

    [Fact]
    public void WithSupplierId_UpdatesSupplierId()
    {
        var product = BuildProduct(supplierId: 1);

        product.WithSupplierId(77);

        product.SupplierId.Should().Be(77);
    }
}
