using Domain.Core;
using MediatR;

namespace Application.Category.Delete;

public sealed record DeleteCategoryRequest(Guid CategoryPublicId)
    : IRequest<Result>;
