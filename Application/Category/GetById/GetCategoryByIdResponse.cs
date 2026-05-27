namespace Application.Category.GetById;

public sealed record GetCategoryByIdResponse(string Name, string? Description, Guid PublicId);
