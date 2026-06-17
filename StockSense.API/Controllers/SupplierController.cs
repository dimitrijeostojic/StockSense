using Application.Constants;
using Application.SupplierManagement.CreateSupplier;
using Application.SupplierManagement.DeleteSupplier;
using Application.SupplierManagement.GetAllSuppliers;
using Application.SupplierManagement.GetSupplierById;
using Application.SupplierManagement.UpdateSupplier;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SupplierController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<IActionResult> GetAllSuppliersAsync([FromQuery] string? searchTerm, [FromQuery] int pageSize, [FromQuery] int pageNumber, [FromQuery] string? sortBy, [FromQuery] bool isAscending, CancellationToken cancellationToken)
    {
        var request = new GetAllSuppliersRequest(searchTerm, sortBy, isAscending, pageSize, pageNumber);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{publicId:Guid}")]
    public async Task<IActionResult> UpdateSupplierAsync([FromRoute] Guid publicId, [FromBody] UpdateSupplierRequestBody requestBody, CancellationToken cancellationToken)
    {
        var request = new UpdateSupplierRequest(publicId, requestBody.Name, requestBody.ContactName, requestBody.ContactEmail, requestBody.ContactPhone);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId:Guid}")]
    public async Task<IActionResult> GetSupplierByIdAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new GetSupplierByIdRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{publicId:Guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteSupplierAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var request = new DeleteSupplierRequest(publicId);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplierAsync([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
