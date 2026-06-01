using Application.Product.Create;
using Application.Product.Delete;
using Application.Product.GetAll;
using Application.Product.GetById;
using Application.Product.Update;
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
    public async Task<IActionResult> GetAllProductsAsync([FromQuery] string searchTerm, [FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] string sortBy, [FromQuery] bool isAscending, CancellationToken cancellationToken)
    {
        var request = new GetAllProductsRequest(searchTerm, sortBy, isAscending, pageSize, pageNumber);
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
}