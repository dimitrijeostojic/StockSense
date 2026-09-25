using Application.UserManagement.CompleteTour;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost("complete/{pageName}")]
    [Authorize]
    public async Task<IActionResult> CompleteTourAsync([FromRoute] string pageName, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteTourRequest(pageName), cancellationToken);
        return result.ToActionResult();
    }
}
