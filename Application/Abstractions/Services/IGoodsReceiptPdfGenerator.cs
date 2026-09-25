using Application.Common.Pdf;

namespace Application.Abstractions.Services;

public interface IGoodsReceiptPdfGenerator
{
    byte[] Generate(GoodsReceiptPdfData data);
}
