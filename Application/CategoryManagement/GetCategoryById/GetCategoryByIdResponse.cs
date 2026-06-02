namespace Application.CategoryManagement.GetCategoryById;

public sealed record GetCategoryByIdResponse(string Name, string? Description, Guid PublicId);
