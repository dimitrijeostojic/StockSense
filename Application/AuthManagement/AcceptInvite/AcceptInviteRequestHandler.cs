using Application.Abstractions.Services;
using Application.Common.Constants;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.AcceptInvite;

internal sealed class AcceptInviteRequestHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork authUnitOfWork,
    ITenantRepository tenantRepository)
    : IRequestHandler<AcceptInviteRequest, TResult<AcceptInviteResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));

    public async Task<TResult<AcceptInviteResponse>> Handle(AcceptInviteRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return TResult<AcceptInviteResponse>.Failure(ApplicationErrors.NotFound);
        }

        var tokenValid = await _userManager.VerifyUserTokenAsync(
            user,
            InviteTokenConstants.ProviderName,
            InviteTokenConstants.TokenPurpose,
            request.Token);

        if (!tokenValid)
        {
            return TResult<AcceptInviteResponse>.Failure(ApplicationErrors.InvalidInviteToken);
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            var errorMessage = string.Join(" ", addPasswordResult.Errors.Select(e => e.Description));
            return TResult<AcceptInviteResponse>.Failure(new Error("AcceptInvite.PasswordFailed", errorMessage));
        }

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant == null)
        {
            return TResult<AcceptInviteResponse>.Failure(ApplicationErrors.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateToken(user, tenant.PublicId, tenant.Name, roles);
        var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<AcceptInviteResponse>.Success(new AcceptInviteResponse(accessToken, refreshToken.Token));
    }
}
