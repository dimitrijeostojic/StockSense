using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.Common.Pdf;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.ExportPDF;

internal sealed class ExportPDFRequestHandler(
    IOrderPdfGenerator pdfGenerator,
    IOrderRepository orderRepository,
    ICurrentUserAccessor currentUserAccessor,
    ITenantRepository tenantRepository
    )
    : IRequestHandler<ExportPDFRequest, TResult<ExportPDFResponse>>
{
    private readonly IOrderPdfGenerator _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));

    public async Task<TResult<ExportPDFResponse>> Handle(ExportPDFRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (order is null)
        {
            return TResult<ExportPDFResponse>.Failure(ApplicationErrors.NotFound);
        }

        var tenant = await _tenantRepository.GetByPublicIdAsync(order.TenantPublicId, cancellationToken);
        if (tenant is null)
        {
            return TResult<ExportPDFResponse>.Failure(ApplicationErrors.NotFound);
        }

        var pdfData = new OrderPdfData(
            order.PublicId.ToString(),
            order.OrderDate,
            tenant.Name,
            tenant.Pib,
            tenant.Address,
            order.Supplier!.Name,
            order.Supplier.ContactEmail,
            order.Supplier.ContactPhone,
            [.. order.OrderItems.Select(oi => new OrderPdfItem(
                oi.Product!.Name,
                oi.Quantity,
                oi.UnitPrice))]);

        var fileContent = _pdfGenerator.Generate(pdfData);
        var fileName = $"Order-{order.PublicId.ToString()[..8]}.pdf";

        return TResult<ExportPDFResponse>.Success(new ExportPDFResponse(fileContent, fileName, "application/pdf"));
    }
}
