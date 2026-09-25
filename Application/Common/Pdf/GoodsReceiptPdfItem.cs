namespace Application.Common.Pdf;

public sealed record GoodsReceiptPdfItem(string Sku, string ProductName, string UnitOfMeasure, int OrderedQuantity, int ReceivedQuantity)
{
    public int Difference => ReceivedQuantity - OrderedQuantity;
}
