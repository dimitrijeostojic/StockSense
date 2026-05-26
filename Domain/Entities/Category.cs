namespace Domain.Entities;

public class Category : AuditableEntity
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    private readonly List<Product> _products = [];

    public static Category CreateCategory(string name, string description)
    {
        return new Category
        {
            Name = name,
            Description = description
        };
    }
}
