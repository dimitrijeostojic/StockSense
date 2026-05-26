namespace Domain.Entities;

public class Supplier : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string ContactName { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string ContactPhone { get; private set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];

    public static Supplier CreateSupplier(string name, string contactName, string contactEmail, string contactPhone)
    {
        return new Supplier
        {
            Name = name,
            ContactName = contactName,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone
        };
    }
}
