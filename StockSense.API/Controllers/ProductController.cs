using Application.ProductManagement.CreateProduct;
using Application.ProductManagement.CreateStockEntry;
using Application.ProductManagement.DeleteProduct;
using Application.ProductManagement.GetAllProducts;
using Application.ProductManagement.GetAllStockEntries;
using Application.ProductManagement.GetCurrentStock;
using Application.ProductManagement.GetProductById;
using Application.ProductManagement.GetStockEntryByProductId;
using Application.ProductManagement.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<IActionResult> GetAllProductsAsync([FromQuery] string? searchTerm, [FromQuery] string? sortBy, [FromQuery] bool isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000, CancellationToken cancellationToken = default)
    {
        var request = new GetAllProductsRequest(searchTerm, sortBy, isAscending, pageNumber, pageSize);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{publicId:Guid}")]
    public async Task<IActionResult> UpdateProductAsync([FromRoute] Guid publicId, [FromBody] UpdateProductRequestBody requestBody, CancellationToken cancellationToken)
    {
        var request = new UpdateProductRequest(publicId, requestBody.Name, requestBody.Description, requestBody.Price, requestBody.MinimumStockQuantity, requestBody.CategoryId, requestBody.SupplierId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId:Guid}")]
    public async Task<IActionResult> GetProductByIdAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new GetProductByIdRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{publicId:Guid}")]
    public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new DeleteProductRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId:Guid}/stockentry")]
    public async Task<IActionResult> GetAllStockEntriesAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new GetAllStockEntriesRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId:Guid}/stockentry/{entryPublicId:Guid}")]
    public async Task<IActionResult> GetStockEntryByIdAsync([FromRoute] Guid publicId, [FromRoute] Guid entryPublicId, CancellationToken cancellationToken)
    {
        var request = new GetStockEntryByIdRequest(publicId, entryPublicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{publicId:Guid}/stockentry")]
    public async Task<IActionResult> CreateStockEntryAsync([FromRoute] Guid publicId, [FromBody] CreateStockEntryRequestBody requestBody, CancellationToken cancellationToken)
    {
        var request = new CreateStockEntryRequest(publicId, requestBody.Quantity, requestBody.Notes, requestBody.StockEntryType);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId:Guid}/stock")]
    public async Task<IActionResult> GetCurrentStockAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new GetCurrentStockRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}