namespace Application.CategoryManagement.UpdateCategory;

public sealed record UpdateCategoryResponse(string Name, string? Description, Guid PublicId);
