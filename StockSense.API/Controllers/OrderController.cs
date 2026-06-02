using Application.Order.Create;
using Application.Order.Delete;
using Application.Order.GetAll;
using Application.Order.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<IActionResult> GetAllOrdersAsync([FromQuery] string searchTerm, [FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] string sortBy, [FromQuery] bool isAscending, CancellationToken cancellationToken)
    {
        var request = new GetAllOrdersRequest(searchTerm, sortBy, isAscending, pageSize, pageNumber);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{publicId:Guid}")]
    public async Task<IActionResult> UpdateOrderAsync([FromRoute] Guid publicId, [FromBody] UpdateOrderRequestBody requestBody, CancellationToken cancellationToken)
    {
        var request = new UpdateOrderRequest(publicId, requestBody.Name, requestBody.ContactName, requestBody.ContactEmail, requestBody.ContactPhone);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId:Guid}")]
    public async Task<IActionResult> GetOrderByIdAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new GetOrderByIdRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{publicId:Guid}")]
    public async Task<IActionResult> DeleteOrderAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new DeleteOrderRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
