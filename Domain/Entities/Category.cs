namespace Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    private readonly List<Product> _products = [];

    private Category()
    {

    }

    public static Category Create(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Category
        {
            Name = name,
            Description = description
        };
    }

    public Category WithName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        return this;
    }

    public Category WithDescription(string? description)
    {
        Description = description;
        return this;
    }
}