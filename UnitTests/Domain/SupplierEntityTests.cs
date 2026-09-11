using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class SupplierEntityTests
{
    private static readonly Guid TenantId = Guid.NewGuid();

    [Fact]
    public void CreateSupplier_WithValidInputs_ReturnsSupplierWithExpectedValues()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "john@acme.com", "SUP-001", "123456", null, null, null, TenantId);

        supplier.Name.Should().Be("Acme");
        supplier.ContactName.Should().Be("John");
        supplier.ContactEmail.Should().Be("john@acme.com");
        supplier.SupplierCode.Should().Be("SUP-001");
        supplier.ContactPhone.Should().Be("123456");
        supplier.TenantPublicId.Should().Be(TenantId);
    }

    [Fact]
    public void CreateSupplier_WithNullOptionalFields_StoresNulls()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "john@acme.com", "SUP-001", null, null, null, null, TenantId);

        supplier.ContactPhone.Should().BeNull();
        supplier.Address.Should().BeNull();
        supplier.City.Should().BeNull();
        supplier.Country.Should().BeNull();
    }

    [Fact]
    public void CreateSupplier_AssignsNewPublicId()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "john@acme.com", "SUP-001", null, null, null, null, TenantId);

        supplier.PublicId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void CreateSupplier_HasEmptyOrdersAndProducts()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "john@acme.com", "SUP-001", null, null, null, null, TenantId);

        supplier.Orders.Should().BeEmpty();
        supplier.Products.Should().BeEmpty();
    }

    [Fact]
    public void WithName_UpdatesName()
    {
        var supplier = Supplier.CreateSupplier("Old", "John", "john@acme.com", "SUP-001", null, null, null, null, TenantId);

        supplier.WithName("New");

        supplier.Name.Should().Be("New");
    }

    [Fact]
    public void WithContactName_UpdatesContactName()
    {
        var supplier = Supplier.CreateSupplier("Acme", "Jane", "jane@acme.com", "SUP-001", null, null, null, null, TenantId);

        supplier.WithContactName("Alice");

        supplier.ContactName.Should().Be("Alice");
    }

    [Fact]
    public void WithContactEmail_UpdatesContactEmail()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "old@mail.com", "SUP-001", null, null, null, null, TenantId);

        supplier.WithContactEmail("new@mail.com");

        supplier.ContactEmail.Should().Be("new@mail.com");
    }

    [Fact]
    public void WithContactPhone_UpdatesContactPhone()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "john@acme.com", "SUP-001", "000", null, null, null, TenantId);

        supplier.WithContactPhone("999");

        supplier.ContactPhone.Should().Be("999");
    }

    [Fact]
    public void WithName_ReturnsSameInstance()
    {
        var supplier = Supplier.CreateSupplier("Acme", "John", "john@acme.com", "SUP-001", null, null, null, null, TenantId);

        var returned = supplier.WithName("Other");

        returned.Should().BeSameAs(supplier);
    }
}
