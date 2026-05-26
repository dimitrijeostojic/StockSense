namespace Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = [];

    public static Category CreateCategory(string name, string description)
    {
        return new Category
        {
            Name = name,
            Description = description
        };
    }
}
