namespace Domain.Entities;

public class Supplier : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? ContactName { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    private readonly List<Product> _products = [];
    public Guid TenantPublicId { get; private set; }

    public static Supplier CreateSupplier(string name, string? contactName, string? contactEmail, string? contactPhone, Guid tenantPublicId)
    {
        return new Supplier
        {
            Name = name,
            ContactName = contactName,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            TenantPublicId = tenantPublicId
        };
    }

    public Supplier WithName(string name)
    {
        Name = name;
        return this;
    }
    public Supplier WithContactName(string? contactName)
    {
        ContactName = contactName;
        return this;
    }
    public Supplier WithContactEmail(string? contactEmail)
    {
        ContactEmail = contactEmail;
        return this;
    }
    public Supplier WithContactPhone(string? contactPhone)
    {
        ContactPhone = contactPhone;
        return this;
    }
}
