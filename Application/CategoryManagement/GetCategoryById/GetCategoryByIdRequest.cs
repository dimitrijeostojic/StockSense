using Domain.Core;
using MediatR;

namespace Application.CategoryManagement.GetCategoryById;

public sealed record GetCategoryByIdRequest(Guid CategoryPublicId)
    : IRequest<TResult<GetCategoryByIdResponse>>;
