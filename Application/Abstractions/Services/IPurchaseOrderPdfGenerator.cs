using Application.Common.Pdf;

namespace Application.Abstractions.Services;

public interface IPurchaseOrderPdfGenerator
{
    byte[] Generate(PurchaseOrderPdfData data);
}
