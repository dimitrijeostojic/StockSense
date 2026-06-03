using Application.Common.Interfaces;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.CategoryManagement.GetAllCategories;

internal sealed class GetAllCategoriesRequestHandler(
    ICategoryRepository categoryRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<GetAllCategoriesRequest, TResult<GetAllCategoriesResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetAllCategoriesResponse>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        var categoryDtos = categories.Select(c => new GetAllCategoriesDto(
            c.Name,
            c.Description,
            c.PublicId
        )).ToList();
        return TResult<GetAllCategoriesResponse>.Success(new GetAllCategoriesResponse(categoryDtos));
    }
}