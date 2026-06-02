using Domain.Core;
using MediatR;

namespace Application.CategoryManagement.UpdateCategory;

public sealed record UpdateCategoryRequest(Guid CategoryPublicId, string Name, string? Description)
    : IRequest<TResult<UpdateCategoryResponse>>;
