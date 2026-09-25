using Application.GoodsReceiptManagement.CreateGoodsReceipt;
using Application.GoodsReceiptManagement.ExportGoodsReceiptPdf;
using Application.GoodsReceiptManagement.GetGoodsReceiptByOrderId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/orders/{orderPublicId:guid}/goods-receipt")]
[ApiController]
[Authorize]
public class GoodsReceiptController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost]
    public async Task<IActionResult> CreateGoodsReceiptAsync(
        [FromRoute] Guid orderPublicId,
        [FromBody] CreateGoodsReceiptRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        var request = new CreateGoodsReceiptRequest(orderPublicId, requestBody.Notes, requestBody.Items);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetGoodsReceiptAsync(
        [FromRoute] Guid orderPublicId,
        CancellationToken cancellationToken)
    {
        var request = new GetGoodsReceiptByOrderIdRequest(orderPublicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("export-pdf")]
    public async Task<ActionResult> ExportGoodsReceiptPdfAsync(
        [FromRoute] Guid orderPublicId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExportGoodsReceiptPdfRequest(orderPublicId), cancellationToken);
        if (!result.IsSuccess)
            return (ActionResult)result.ToActionResult();

        return File(result.Value.FileContent, result.Value.MimeType, result.Value.FileName);
    }
}
