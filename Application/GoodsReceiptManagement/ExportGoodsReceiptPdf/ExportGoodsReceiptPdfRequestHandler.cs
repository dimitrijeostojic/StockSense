using Application.Abstractions.Services;
using Application.Common.Errors;
using Application.Common.Pdf;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.GoodsReceiptManagement.ExportGoodsReceiptPdf;

internal sealed class ExportGoodsReceiptPdfRequestHandler(
    IGoodsReceiptPdfGenerator pdfGenerator,
    IGoodsReceiptRepository goodsReceiptRepository,
    IOrderRepository orderRepository,
    ITenantRepository tenantRepository,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<ExportGoodsReceiptPdfRequest, TResult<ExportGoodsReceiptPdfResponse>>
{
    private readonly IGoodsReceiptPdfGenerator _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
    private readonly IGoodsReceiptRepository _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<ExportGoodsReceiptPdfResponse>> Handle(ExportGoodsReceiptPdfRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;

        var receipt = await _goodsReceiptRepository.GetByOrderPublicIdAsync(request.OrderPublicId, tenantPublicId, cancellationToken);
        if (receipt is null)
            return TResult<ExportGoodsReceiptPdfResponse>.Failure(ApplicationErrors.NotFound);

        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, tenantPublicId, cancellationToken);
        if (order is null)
            return TResult<ExportGoodsReceiptPdfResponse>.Failure(ApplicationErrors.NotFound);

        var tenant = await _tenantRepository.GetByPublicIdAsync(tenantPublicId, cancellationToken);
        if (tenant is null)
            return TResult<ExportGoodsReceiptPdfResponse>.Failure(ApplicationErrors.NotFound);

        var pdfData = new GoodsReceiptPdfData(
            order.PublicId.ToString(),
            order.OrderDate,
            receipt.ReceivedAt,
            tenant.Name,
            tenant.Pib,
            tenant.Address,
            tenant.Logo,
            order.Supplier!.Name,
            order.Supplier.Address,
            order.Supplier.City,
            order.Supplier.Country,
            receipt.Notes,
            order.Currency.ToString(),
            [..receipt.Items.Select(i => new GoodsReceiptPdfItem(
                i.OrderItem!.Product!.Sku,
                i.OrderItem.Product.Name,
                i.OrderItem.Product.UnitOfMeasure.ToString(),
                i.OrderedQuantity,
                i.ReceivedQuantity))]);

        var fileContent = _pdfGenerator.Generate(pdfData);
        var fileName = $"GoodsReceipt-{order.PublicId.ToString()[..8]}.pdf";

        return TResult<ExportGoodsReceiptPdfResponse>.Success(new ExportGoodsReceiptPdfResponse(fileContent, fileName, "application/pdf"));
    }
}
