using Application.AuthManagement.ChangePassword;
using Application.AuthManagement.ForgotPassword;
using Application.AuthManagement.Login;
using Application.AuthManagement.Logout;
using Application.AuthManagement.RefreshToken;
using Application.AuthManagement.Register;
using Application.AuthManagement.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost("register")]
    [EnableRateLimiting("Auth")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterAsync(
        [FromForm] string firstName,
        [FromForm] string lastName,
        [FromForm] string username,
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string companyName,
        [FromForm] string pib,
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

        var request = new RegisterRequest(firstName, lastName, username, email, password, companyName, pib, address, logoBytes);
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    [EnableRateLimiting("Auth")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("Auth")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("Auth")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("Auth")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
