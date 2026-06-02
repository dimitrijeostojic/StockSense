using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.ProductManagement.CreateStockEntry;

internal sealed class CreateStockEntryRequestHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
    )
    : IRequestHandler<CreateStockEntryRequest, TResult<CreateStockEntryResponse>>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<TResult<CreateStockEntryResponse>> Handle(CreateStockEntryRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByPublicIdAsync(request.ProductPublicId, cancellationToken);
        if (product == null)
        {
            return TResult<CreateStockEntryResponse>.Failure(ApplicationErrors.NotFound);
        }

        var entry = product.AddStockEntry(request.Quantity, request.StockEntryType, request.Notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<CreateStockEntryResponse>.Success(new CreateStockEntryResponse(entry.PublicId, entry.Quantity, entry.EntryDate, entry.Notes, entry.StockEntryType, product.PublicId, product.Name));
    }
}
