using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.Common.Pdf;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.ExportPurchaseOrderPdf;

internal sealed class ExportPurchaseOrderPdfRequestHandler(
    IPurchaseOrderPdfGenerator pdfGenerator,
    IOrderRepository orderRepository,
    ICurrentUserAccessor currentUserAccessor,
    ITenantRepository tenantRepository
    )
    : IRequestHandler<ExportPurchaseOrderPdfRequest, TResult<ExportPurchaseOrderPdfResponse>>
{
    private readonly IPurchaseOrderPdfGenerator _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));

    public async Task<TResult<ExportPurchaseOrderPdfResponse>> Handle(ExportPurchaseOrderPdfRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (order is null)
        {
            return TResult<ExportPurchaseOrderPdfResponse>.Failure(ApplicationErrors.NotFound);
        }

        var tenant = await _tenantRepository.GetByPublicIdAsync(order.TenantPublicId, cancellationToken);
        if (tenant is null)
        {
            return TResult<ExportPurchaseOrderPdfResponse>.Failure(ApplicationErrors.NotFound);
        }

        var pdfData = new PurchaseOrderPdfData(
            order.PublicId.ToString(),
            order.OrderDate,
            tenant.Name,
            tenant.Pib,
            tenant.Address,
            tenant.Logo,
            order.Supplier!.Name,
            order.Supplier.Address,
            order.Supplier.City,
            order.Supplier.Country,
            order.Notes,
            order.Currency.ToString(),
            [..order.OrderItems.Select(oi => new PurchaseOrderPdfItem(
                oi.Product!.Sku,
                oi.Product.Name,
                order.Supplier.SupplierCode,
                oi.Product.UnitOfMeasure.ToString(),
                oi.Quantity,
                oi.UnitPrice,
                oi.VatRate))]);

        var fileContent = _pdfGenerator.Generate(pdfData);
        var fileName = $"PurchaseOrder-{order.PublicId.ToString()[..8]}.pdf";

        return TResult<ExportPurchaseOrderPdfResponse>.Success(new ExportPurchaseOrderPdfResponse(fileContent, fileName, "application/pdf"));
    }
}
