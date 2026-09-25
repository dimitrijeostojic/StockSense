using Domain.Core;
using MediatR;

namespace Application.GoodsReceiptManagement.ExportGoodsReceiptPdf;

public sealed record ExportGoodsReceiptPdfRequest(Guid OrderPublicId) : IRequest<TResult<ExportGoodsReceiptPdfResponse>>;
