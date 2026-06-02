using Domain.Core;
using Domain.Enums;
using MediatR;

namespace Application.ProductManagement.CreateStockEntry;

public sealed record CreateStockEntryRequest(
    Guid ProductPublicId,
    int Quantity,
    string? Notes,
    StockEntryType StockEntryType
   )
    : IRequest<TResult<CreateStockEntryResponse>>;
