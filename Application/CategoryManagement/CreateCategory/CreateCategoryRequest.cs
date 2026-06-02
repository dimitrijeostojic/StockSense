using Domain.Core;
using MediatR;

namespace Application.CategoryManagement.CreateCategory;

public sealed record CreateCategoryRequest(string Name, string Description)
    : IRequest<TResult<CreateCategoryResponse>>;
