namespace Application.Common.Pdf;

public sealed record OrderPdfItem(string Sku, string ProductName, string SupplierCode, string UnitOfMeasure, int Quantity, decimal UnitPrice, decimal VatRate)
{
    public decimal NetTotal => Quantity * UnitPrice;
    public decimal VatAmount => NetTotal * (VatRate / 100);
}
