using Domain.Enums;

namespace Application.ProductManagement.GetStockEntryByProductId;

public sealed record GetStockEntryByIdResponse(
    Guid PublicId,
    int Quantity,
    DateTime EntryDate,
    string? Notes,
    StockEntryType StockEntryType,
    Guid ProductPublicId,
    string ProductName
    );
