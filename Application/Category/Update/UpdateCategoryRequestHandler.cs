using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Category.Update;

internal sealed class UpdateCategoryRequestHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCategoryRequest, Result<UpdateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result<UpdateCategoryResponse>> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, cancellationToken);
        if (category == null)
        {
            return Result<UpdateCategoryResponse>.Failure(CategoryErrors.NotFound);
        }
        category = category.WithName(request.Name)
            .WithDescription(request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UpdateCategoryResponse>.Success(new UpdateCategoryResponse(category.Name, category.Description, category.PublicId));
    }
}
