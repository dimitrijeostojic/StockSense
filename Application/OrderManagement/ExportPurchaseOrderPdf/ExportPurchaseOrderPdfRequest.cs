using Domain.Core;
using MediatR;

namespace Application.OrderManagement.ExportPurchaseOrderPdf;

public sealed record ExportPurchaseOrderPdfRequest(Guid OrderPublicId) : IRequest<TResult<ExportPurchaseOrderPdfResponse>>;
