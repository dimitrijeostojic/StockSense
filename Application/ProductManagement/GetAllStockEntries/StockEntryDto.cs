using Domain.Enums;

namespace Application.ProductManagement.GetAllStockEntries;

public sealed record StockEntryDto(
    Guid PublicId,
    int Quantity,
    DateTime EntryDate,
    string? Notes,
    StockEntryType StockEntryType,
    Guid ProductPublicId,
    string ProductName
    );
