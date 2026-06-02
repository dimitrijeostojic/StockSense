using Application.Common.Collections;

namespace Application.ProductManagement.GetAllStockEntries;

public sealed class GetAllStockEntriesResponse(
    ICollection<StockEntryDto> Items) : EntityCollectionResult<StockEntryDto>(Items);
