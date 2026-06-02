using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.CreateProduct;

internal sealed class CreateProductRequestHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork
    )
    : IRequestHandler<CreateProductRequest, TResult<CreateProductResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<CreateProductResponse>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByPublicIdAsync(request.CategoryPublicId, cancellationToken);
        var supplier = await _supplierRepository.GetByPublicIdAsync(request.SupplierPublicId, cancellationToken);
        if (category == null || supplier == null)
        {
            return TResult<CreateProductResponse>.Failure(ApplicationErrors.NotFound);
        }

        var product = Domain.Entities.Product.CreateProduct(
            request.Name,
            request.Description,
            request.Price,
            request.MinimumStockQuantity,
            category.Id,
            supplier.Id);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<CreateProductResponse>.Success(new CreateProductResponse(
            product.PublicId,
            product.Name,
            product.Description,
            product.Price,
            product.MinimumStockQuantity,
            category.PublicId,
            supplier.PublicId));
    }
}
