namespace Application.CategoryManagement.GetAllCategories;

public sealed record GetAllCategoriesDto(string Name, string? Description, Guid PublicId);
