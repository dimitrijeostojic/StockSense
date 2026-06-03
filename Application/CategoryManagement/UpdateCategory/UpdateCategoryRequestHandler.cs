using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.CategoryManagement.UpdateCategory;

internal sealed class UpdateCategoryRequestHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<UpdateCategoryRequest, TResult<UpdateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<UpdateCategoryResponse>> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (category == null)
        {
            return TResult<UpdateCategoryResponse>.Failure(ApplicationErrors.NotFound);
        }
        category = category.WithName(request.Name)
            .WithDescription(request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return TResult<UpdateCategoryResponse>.Success(new UpdateCategoryResponse(category.Name, category.Description, category.PublicId));
    }
}
