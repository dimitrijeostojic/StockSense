using Application.Abstractions.Services;
using Application.Common.Constants;
using Application.Common.Errors;
using Application.Common.Options;
using Application.Emails;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UserManagement.ResendInvite;

internal sealed class ResendInviteRequestHandler(
    ICurrentUserAccessor currentUserAccessor,
    IUserRepository userRepository,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IOptions<AppOptions> options,
    ILogger<ResendInviteRequestHandler> logger)
    : IRequestHandler<ResendInviteRequest, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly AppOptions _options = options.Value;
    private readonly ILogger<ResendInviteRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result> Handle(ResendInviteRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByPublicIdAsync(
            request.UserPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure(ApplicationErrors.UserAlreadyActive);
        }

        var token = await _userManager.GenerateUserTokenAsync(user, InviteTokenConstants.ProviderName, InviteTokenConstants.TokenPurpose);
        var encodedToken = Uri.EscapeDataString(token);
        var inviteLink = $"{_options.FrontendBaseUrl}/accept-invite?token={encodedToken}&email={Uri.EscapeDataString(user.Email!)}";

        try
        {
            var message = EmailTemplates.UserInvited(user.Email!, user.FirstName ?? "there", user.Tenant?.Name ?? string.Empty, inviteLink);
            await _emailService.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resend invite email to {Email}", user.Email);
        }

        return Result.Success();
    }
}
