namespace Application.CategoryManagement.CreateCategory;

public sealed record CreateCategoryResponse(string Name, string? Description, Guid PublicId);
