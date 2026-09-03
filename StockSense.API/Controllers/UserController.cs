using Application.Common.Constants;
using Application.UserManagement.Delete;
using Application.UserManagement.GetAll;
using Application.UserManagement.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    [Route("GetAllUsers")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllUsersRequest(), cancellationToken);
        return response.ToActionResult();
    }

    [HttpDelete]
    [Route("delete-user/{userPublicId:Guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid userPublicId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new DeleteUserRequest { UserPublicId = userPublicId }, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPost("register-user")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
