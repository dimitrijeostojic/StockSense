using Application.Common.Pdf;

namespace Application.Abstractions.Services;

public interface IOrderPdfGenerator
{
    byte[] Generate(OrderPdfData data);
}
