using Domain.Enums;

namespace Domain.Entities;

public class StockEntry : Entity
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime EntryDate { get; private set; }
    public string? Notes { get; private set; }
    public StockEntryType StockEntryType { get; private set; }
    public Product? Product { get; private set; }

    internal static StockEntry Create(int productId, int quantity, DateTime entryDate, string notes, StockEntryType stockEntryType)
    {
        return new StockEntry
        {
            ProductId = productId,
            Quantity = quantity,
            EntryDate = entryDate,
            Notes = notes,
            StockEntryType = stockEntryType
        };
    }
}
