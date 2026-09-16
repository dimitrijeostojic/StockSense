using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.CategoryManagement.DeleteCategory;

internal sealed class DeleteCategoryRequestHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor,
    IProductRepository productRepository
    )
    : IRequestHandler<DeleteCategoryRequest, Result>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<Result> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (category == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        var hasProducts = await _productRepository.AnyByCategoryIdAsync(category.Id, cancellationToken);
        if (hasProducts)
        {
            return Result.Failure(ApplicationErrors.CategoryHasProducts);
        }
        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
