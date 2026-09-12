using Domain.Primitives;

namespace Domain.Entities;

public class Supplier : AggregateRoot
{
    public string Name { get; private set; }
    public string ContactName { get; private set; }
    public string ContactEmail { get; private set; }
    public string SupplierCode { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }
    public IReadOnlyCollection<Order> Orders => _orders;
    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Product> Products => _products;
    private readonly List<Product> _products = [];
    public Guid TenantPublicId { get; private set; }

    private Supplier()
    {
        Name = string.Empty;
        ContactName = string.Empty;
        ContactEmail = string.Empty;
        SupplierCode = string.Empty;
    }

    private Supplier(string name, string contactName, string contactEmail, string supplierCode, string? contactPhone, string? address, string? city, string? country, Guid tenantPublicId)
    {
        Name = name;
        ContactName = contactName;
        ContactEmail = contactEmail;
        SupplierCode = supplierCode;
        ContactPhone = contactPhone;
        Address = address;
        City = city;
        Country = country;
        TenantPublicId = tenantPublicId;
    }


    public static Supplier CreateSupplier(string name, string contactName, string contactEmail, string supplierCode, string? contactPhone, string? address, string? city, string? country, Guid tenantPublicId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(contactName);
        ArgumentNullException.ThrowIfNullOrEmpty(contactEmail);
        ArgumentNullException.ThrowIfNullOrEmpty(supplierCode);
        return new Supplier(name, contactName, contactEmail, supplierCode, contactPhone, address, city, country, tenantPublicId);
    }

    public Supplier WithName(string name)
    {
        Name = name;
        return this;
    }
    public Supplier WithContactName(string contactName)
    {
        ContactName = contactName;
        return this;
    }
    public Supplier WithContactEmail(string contactEmail)
    {
        ContactEmail = contactEmail;
        return this;
    }
    public Supplier WithSupplierCode(string supplierCode)
    {
        SupplierCode = supplierCode;
        return this;
    }
    public Supplier WithContactPhone(string? contactPhone)
    {
        ContactPhone = contactPhone;
        return this;
    }
    public Supplier WithAddress(string? address)
    {
        Address = address;
        return this;
    }
    public Supplier WithCity(string? city)
    {
        City = city;
        return this;
    }
    public Supplier WithCountry(string? country)
    {
        Country = country;
        return this;
    }
}
