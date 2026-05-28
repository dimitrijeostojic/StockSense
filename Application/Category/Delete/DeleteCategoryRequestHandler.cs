using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Category.Delete;

internal sealed class DeleteCategoryRequestHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
    )
    : IRequestHandler<DeleteCategoryRequest, Result>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, cancellationToken);
        if (category == null)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }
        _categoryRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
