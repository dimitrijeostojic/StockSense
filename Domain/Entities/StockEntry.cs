using Domain.Enums;

namespace Domain.Entities;

public class StockEntry : AuditableEntity
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime EntryDate { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public StockEntryType StockEntryType { get; private set; }
    public Product? Product { get; private set; }
}
