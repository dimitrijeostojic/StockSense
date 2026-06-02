using Domain.Core;
using MediatR;

namespace Application.CategoryManagement.DeleteCategory;

public sealed record DeleteCategoryRequest(Guid CategoryPublicId)
    : IRequest<Result>;
