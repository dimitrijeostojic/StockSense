namespace Application.Common.Pdf;

public sealed record OrderPdfItem(string ProductName, int Quantity, decimal UnitPrice)
{
    public decimal Total => Quantity * UnitPrice;
}
