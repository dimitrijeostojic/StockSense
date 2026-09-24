using Application.AnalyticsManagement.Common;
using Application.AnalyticsManagement.GetBusinessAnalytics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AnalyticsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("business")]
    public async Task<IActionResult> GetBusinessAnalytics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 5,
        [FromQuery] int stockPage = 1,
        [FromQuery] int stockPageSize = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetBusinessAnalyticsRequest(new TimeRangeQuery(from, to), topN, stockPage, stockPageSize),
            cancellationToken);
        return result.ToActionResult();
    }
}
