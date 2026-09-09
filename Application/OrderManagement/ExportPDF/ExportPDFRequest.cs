using Domain.Core;
using MediatR;

namespace Application.OrderManagement.ExportPDF;

public sealed record ExportPDFRequest(Guid OrderPublicId) : IRequest<TResult<ExportPDFResponse>>;
