using Application.Abstractions.Idempotency;
using Domain.Core;
using Domain.Enums;

namespace Application.ProductManagement.CreateStockEntry;

public sealed record CreateStockEntryRequest(
    Guid RequestId,
    Guid ProductPublicId,
    int Quantity,
    string? Notes,
    StockEntryType StockEntryType
   )
    : IdempotentRequest<TResult<CreateStockEntryResponse>>(RequestId);
