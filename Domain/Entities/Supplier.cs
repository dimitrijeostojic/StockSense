namespace Domain.Entities;

public class Supplier : AuditableEntity
{
    public string? Name { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    private readonly List<Product> _products = [];

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
