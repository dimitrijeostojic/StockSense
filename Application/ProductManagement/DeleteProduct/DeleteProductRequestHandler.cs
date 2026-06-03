using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.DeleteProduct;

internal sealed class DeleteProductRequestHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<DeleteProductRequest, Result>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    public async Task<Result> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.PublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (product == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
