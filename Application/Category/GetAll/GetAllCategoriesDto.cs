namespace Application.Category.GetAll;

public sealed class GetAllCategoriesDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid PublicId { get; set; }
}
