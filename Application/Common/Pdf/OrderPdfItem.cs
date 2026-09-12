namespace Application.Common.Pdf;

public sealed record OrderPdfItem(string Sku, string ProductName, string SupplierCode, string UnitOfMeasure, int Quantity, decimal UnitPrice)
{
    public decimal Total => Quantity * UnitPrice;
}
