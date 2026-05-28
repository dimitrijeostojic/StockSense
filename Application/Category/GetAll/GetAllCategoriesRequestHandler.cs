using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Category.GetAll;

internal sealed class GetAllCategoriesRequestHandler(
    ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllCategoriesRequest, TResult<GetAllCategoriesResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

    public async Task<TResult<GetAllCategoriesResponse>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoryDtos = categories.Select(c => new GetAllCategoriesDto(
            c.Name,
            c.Description,
            c.PublicId
        )).ToList();
        return TResult<GetAllCategoriesResponse>.Success(new GetAllCategoriesResponse(categoryDtos));
    }
}