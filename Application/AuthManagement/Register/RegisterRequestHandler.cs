using Application.Abstractions.Services;
using Application.Common.Constants;
using Application.Common.Errors;
using Application.Emails;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.AuthManagement.Register;

internal sealed class RegisterRequestHandler
    (UserManager<ApplicationUser> userManager,
    ILogger<RegisterRequestHandler> logger,
    ITenantRepository tenantRepository,
    IAuthUnitOfWork authUnitOfWork,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IEmailService emailService)
        : IRequestHandler<RegisterRequest, TResult<RegisterResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly ILogger<RegisterRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

    public async Task<TResult<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            return TResult<RegisterResponse>.Failure(ApplicationErrors.EmailAlreadyExists);
        }

        var existingTenant = await _tenantRepository.GetByPIBAsync(request.PIB, cancellationToken);
        if (existingTenant != null)
        {
            return TResult<RegisterResponse>.Failure(ApplicationErrors.PIBAlreadyExists);
        }

        using var transaction = _authUnitOfWork.BeginTransaction();
        try
        {

            var tenant = Tenant.Create(request.CompanyName, request.PIB, request.Address);
            await _tenantRepository.AddAsync(tenant, cancellationToken);
            await _authUnitOfWork.SaveChangesAsync(cancellationToken);

            user = ApplicationUser.Create(request.Username, request.Email, request.FirstName, request.LastName, tenant.Id);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return TResult<RegisterResponse>.Failure(ApplicationErrors.RegistrationFailed);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Admin);
            if (!roleResult.Succeeded)
            {
                return TResult<RegisterResponse>.Failure(ApplicationErrors.RegistrationFailed);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.GenerateToken(user, tenant.PublicId, tenant.Name, roles);
            var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _authUnitOfWork.SaveChangesAsync(cancellationToken);

            transaction.Commit();

            try
            {
                var message = EmailTemplates.Welcome(request.Email, request.FirstName, request.CompanyName);
                await _emailService.SendAsync(message, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to queue welcome email for {Email}", request.Email);
            }

            return TResult<RegisterResponse>.Success(new RegisterResponse(accessToken, refreshToken.Token));

        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
