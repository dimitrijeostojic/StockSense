using Application.AnalyticsManagement.Common;
using Application.AnalyticsManagement.GetBusinessAnalytics;
using Application.DashboardManagement.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardRequest(), cancellationToken);
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
