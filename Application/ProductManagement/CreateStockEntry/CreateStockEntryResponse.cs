using Domain.Enums;

namespace Application.ProductManagement.CreateStockEntry;

public sealed record CreateStockEntryResponse(
    Guid PublicId,
    int Quantity,
    DateTime EntryDate,
    string? Notes,
    StockEntryType StockEntryType,
    Guid ProductPublicId,
    string ProductName
 );