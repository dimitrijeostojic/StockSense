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

namespace Application.TenantManagement.CreateTenant;

internal sealed class CreateTenantRequestHandler(
    ITenantRepository tenantRepository,
    UserManager<ApplicationUser> userManager,
    IAuthUnitOfWork authUnitOfWork,
    IEmailService emailService,
    IOptions<AppOptions> options,
    ILogger<CreateTenantRequestHandler> logger)
    : IRequestHandler<CreateTenantRequest, TResult<CreateTenantResponse>>
{
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly AppOptions _options = options.Value;
    private readonly ILogger<CreateTenantRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<TResult<CreateTenantResponse>> Handle(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.AdminEmail);
        if (existingUser != null)
        {
            return TResult<CreateTenantResponse>.Failure(ApplicationErrors.EmailAlreadyExists);
        }

        var existingTenant = await _tenantRepository.GetByPibAsync(request.Pib, cancellationToken);
        if (existingTenant != null)
        {
            return TResult<CreateTenantResponse>.Failure(ApplicationErrors.PIBAlreadyExists);
        }

        using var transaction = _authUnitOfWork.BeginTransaction();
        try
        {
            var tenant = Tenant.Create(request.TenantName, request.Pib, request.Address);
            await _tenantRepository.AddAsync(tenant, cancellationToken);
            await _authUnitOfWork.SaveChangesAsync(cancellationToken);

            var admin = ApplicationUser.Create(
                request.AdminEmail,
                request.AdminEmail,
                request.AdminFirstName,
                request.AdminLastName,
                tenant.Id);

            var createResult = await _userManager.CreateAsync(admin);
            if (!createResult.Succeeded)
            {
                return TResult<CreateTenantResponse>.Failure(ApplicationErrors.RegistrationFailed);
            }

            var roleResult = await _userManager.AddToRoleAsync(admin, Roles.Admin);
            if (!roleResult.Succeeded)
            {
                return TResult<CreateTenantResponse>.Failure(ApplicationErrors.RegistrationFailed);
            }

            await _authUnitOfWork.SaveChangesAsync(cancellationToken);
            transaction.Commit();

            var token = await _userManager.GenerateUserTokenAsync(admin, InviteTokenConstants.ProviderName, InviteTokenConstants.TokenPurpose);
            var encodedToken = Uri.EscapeDataString(token);
            var inviteLink = $"{_options.FrontendBaseUrl}/accept-invite?token={encodedToken}&email={Uri.EscapeDataString(admin.Email!)}";

            try
            {
                var message = EmailTemplates.UserInvited(admin.Email!, request.AdminFirstName, tenant.Name, inviteLink);
                await _emailService.SendAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send invite email to tenant admin {Email}", admin.Email);
            }

            return TResult<CreateTenantResponse>.Success(
                new CreateTenantResponse(tenant.PublicId, tenant.Name, admin.Email!));
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
