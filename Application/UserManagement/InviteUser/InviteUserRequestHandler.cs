using Application.Abstractions.Services;
using Application.Common.Constants;
using Application.Common.Errors;
using Application.Common.Options;
using Application.Emails;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UserManagement.InviteUser;

internal sealed class InviteUserRequestHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITenantRepository tenantRepository,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IOptions<AppOptions> options,
    ILogger<InviteUserRequestHandler> logger)
    : IRequestHandler<InviteUserRequest, TResult<InviteUserResponse>>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly AppOptions _options = options.Value;
    private readonly ILogger<InviteUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<TResult<InviteUserResponse>> Handle(InviteUserRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByPublicIdAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        if (tenant == null)
        {
            return TResult<InviteUserResponse>.Failure(ApplicationErrors.NotFound);
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return TResult<InviteUserResponse>.Failure(ApplicationErrors.EmailAlreadyExists);
        }

        var user = ApplicationUser.Create(request.Email, request.Email, request.FirstName, request.LastName, tenant.Id);
        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            return TResult<InviteUserResponse>.Failure(new Error("InviteUser.CreateFailed",
                string.Join(", ", createResult.Errors.Select(e => e.Description))));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return TResult<InviteUserResponse>.Failure(new Error("InviteUser.RoleFailed",
                string.Join(", ", roleResult.Errors.Select(e => e.Description))));
        }

        var token = await _userManager.GenerateUserTokenAsync(user, InviteTokenConstants.ProviderName, InviteTokenConstants.TokenPurpose);
        var encodedToken = Uri.EscapeDataString(token);
        var inviteLink = $"{_options.FrontendBaseUrl}/accept-invite?token={encodedToken}&email={Uri.EscapeDataString(user.Email!)}";

        try
        {
            var message = EmailTemplates.UserInvited(user.Email!, request.FirstName, tenant.Name, inviteLink);
            await _emailService.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send invite email to {Email}", user.Email);
        }

        return TResult<InviteUserResponse>.Success(new InviteUserResponse("Invite sent successfully."));
    }
}
