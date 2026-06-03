using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.UpdateProduct;

internal sealed class UpdateProductRequestHandler(
    IProductRepository productRepository,
    ISupplierRepository supplierRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor
    )
    : IRequestHandler<UpdateProductRequest, TResult<UpdateProductResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<UpdateProductResponse>> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryId, _currentUserAccessor.TenantPublicId, cancellationToken);
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierId, cancellationToken);
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, cancellationToken);
        if (product == null || category == null || supplier == null)
        {
            return TResult<UpdateProductResponse>.Failure(ApplicationErrors.NotFound);
        }
        product = product.WithName(request.Name)
            .WithDescription(request.Description)
            .WithPrice(request.Price)
            .WithMinimumStockQuantity(request.MinimumStockQuantity)
            .WithCategoryId(category.Id)
            .WithSupplierId(supplier.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var response = new UpdateProductResponse(
            product.PublicId,
            product.Name,
            product.Description,
            product.Price,
            product.MinimumStockQuantity,
            category.PublicId,
            category.Name,
            supplier.PublicId,
            supplier.Name
            );
        return TResult<UpdateProductResponse>.Success(response);

    }
}
