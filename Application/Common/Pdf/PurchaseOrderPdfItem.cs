namespace Application.Common.Pdf;

public sealed record PurchaseOrderPdfItem(string Sku, string ProductName, string SupplierCode, string UnitOfMeasure, int Quantity, decimal UnitPrice, decimal VatRate)
{
    public decimal NetTotal => Quantity * UnitPrice;
    public decimal VatAmount => NetTotal * (VatRate / 100);
}
