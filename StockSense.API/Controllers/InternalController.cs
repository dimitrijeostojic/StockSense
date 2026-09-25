using Application.TenantManagement.CreateTenant;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;
using StockSense.API.Filters;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[RequirePlatformKey]
public class InternalController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost("tenants")]
    public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
