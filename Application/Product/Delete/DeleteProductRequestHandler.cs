using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Product.Delete;

internal sealed class DeleteProductRequestHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductRequest, Result>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.PublicId, cancellationToken);
        if (product == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
