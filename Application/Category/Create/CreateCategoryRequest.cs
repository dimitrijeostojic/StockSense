using Domain.Core;
using MediatR;

namespace Application.Category.Create;

public sealed record CreateCategoryRequest(string Name, string Description)
    : IRequest<TResult<CreateCategoryResponse>>;
