using Domain.Core;
using MediatR;

namespace Application.Category.Update;

public sealed record UpdateCategoryRequest(Guid CategoryPublicId, string Name, string? Description)
    : IRequest<TResult<UpdateCategoryResponse>>;
