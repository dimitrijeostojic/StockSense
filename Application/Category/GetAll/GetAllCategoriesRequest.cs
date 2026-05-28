using Domain.Core;
using MediatR;

namespace Application.Category.GetAll;

public sealed record GetAllCategoriesRequest()
    : IRequest<TResult<GetAllCategoriesResponse>>;
