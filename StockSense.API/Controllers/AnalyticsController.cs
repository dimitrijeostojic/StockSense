using Application.AnalyticsManagement.Common;
using Application.AnalyticsManagement.GetBusinessAnalytics;
using Application.AnalyticsManagement.GetUserAnalytics;
using Application.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class AnalyticsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("users")]
    public async Task<IActionResult> GetUserAnalytics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetUserAnalyticsRequest(new TimeRangeQuery(from, to), topN),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("business")]
    public async Task<IActionResult> GetBusinessAnalytics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetBusinessAnalyticsRequest(new TimeRangeQuery(from, to), topN),
            cancellationToken);
        return result.ToActionResult();
    }
}
