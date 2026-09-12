using Application.Common.Constants;
using Application.TenantManagement.GetMyTenant;
using Application.TenantManagement.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TenantController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<IActionResult> GetMyTenantAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyTenantRequest(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [Authorize(Roles = Roles.Admin)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateTenantAsync(
        [FromForm] string name,
        [FromForm] string? address,
        IFormFile? logo,
        CancellationToken cancellationToken)
    {
        byte[]? logoBytes = null;
        if (logo is not null)
        {
            using var ms = new MemoryStream();
            await logo.CopyToAsync(ms, cancellationToken);
            logoBytes = ms.ToArray();
        }

        var request = new UpdateTenantRequest(name, address, logoBytes);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
