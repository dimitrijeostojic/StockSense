using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.CategoryManagement.CreateCategory;

internal sealed class CreateCategoryRequestHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryRequest, TResult<CreateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<CreateCategoryResponse>> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Category category = Domain.Entities.Category.Create(request.Name, request.Description);
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return TResult<CreateCategoryResponse>.Success(new CreateCategoryResponse(category.Name, category.Description, category.PublicId));
    }
}
