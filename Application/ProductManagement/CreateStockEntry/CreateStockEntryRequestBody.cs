using Domain.Enums;

namespace Application.ProductManagement.CreateStockEntry;

public sealed record CreateStockEntryRequestBody(
    int Quantity,
    string? Notes,
    StockEntryType StockEntryType
   );
