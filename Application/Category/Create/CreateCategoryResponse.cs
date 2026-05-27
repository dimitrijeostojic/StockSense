namespace Application.Category.Create;

public sealed record CreateCategoryResponse(string Name, string? Description, Guid PublicId);
