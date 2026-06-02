using Domain.Core;
using MediatR;

namespace Application.CategoryManagement.GetAllCategories;

public sealed record GetAllCategoriesRequest()
    : IRequest<TResult<GetAllCategoriesResponse>>;
