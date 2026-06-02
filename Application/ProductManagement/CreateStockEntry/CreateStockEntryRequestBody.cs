using Domain.Core;
using Domain.Enums;
using MediatR;

namespace Application.ProductManagement.CreateStockEntry;

public sealed record CreateStockEntryRequestBody(
    int Quantity,
    string? Notes,
    StockEntryType StockEntryType
   )
