using Application.Abstractions.Services;
using Application.Common.Options;
using Application.Emails;
using Domain.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.AuthManagement.ForgotPassword;

internal sealed class ForgotPasswordRequestHandler(
    UserManager<ApplicationUser> userManager,
    IOptions<AppOptions> options,
    IEmailService emailService,
    ILogger<ForgotPasswordRequestHandler> logger)
    : IRequestHandler<ForgotPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly ILogger<ForgotPasswordRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly AppOptions _options = options.Value;

    public async Task<Result> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token); // token sadrži specijalne karaktere, MORA biti URL-encoded
            var resetLink = $"{_options.FrontendBaseUrl}/reset-password?token={encodedToken}&email={Uri.EscapeDataString(user.Email!)}";
            try
            {
                var message = EmailTemplates.PasswordReset(user.Email!, user.FirstName ?? "there", resetLink);
                await _emailService.SendAsync(message, cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to queue password reset email for {Email}", request.Email);
            }
        }
        return Result.Success();
    }
}
