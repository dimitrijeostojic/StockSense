using Domain.Core;
using MediatR;

namespace Application.Category.GetById;

public sealed record GetCategoryByIdRequest(Guid CategoryPublicId)
    : IRequest<Result<GetCategoryByIdResponse>>;
