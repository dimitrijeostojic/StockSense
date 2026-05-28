using Application.Common.Errors;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Category.GetById;

internal sealed class GetCategoryByIdRequestHandler(
    ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryByIdRequest, TResult<GetCategoryByIdResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

    public async Task<TResult<GetCategoryByIdResponse>> Handle(GetCategoryByIdRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, cancellationToken);
        if (category == null)
        {
            return TResult<GetCategoryByIdResponse>.Failure(CategoryErrors.NotFound);
        }
        return TResult<GetCategoryByIdResponse>.Success(new GetCategoryByIdResponse(category.Name, category.Description, category.PublicId));
    }
}
