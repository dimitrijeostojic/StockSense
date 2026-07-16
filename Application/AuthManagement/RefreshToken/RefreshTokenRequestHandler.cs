using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.RefreshToken;

internal sealed class RefreshTokenRequestHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork authUnitOfWork,
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    ITenantRepository tenantRepository
    )
    : IRequestHandler<RefreshTokenRequest, TResult<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));

    public async Task<TResult<RefreshTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingRefreshToken is null || existingRefreshToken.IsRevoked || existingRefreshToken.ExpiresOnUtc <= DateTime.UtcNow)
        {
            return TResult<RefreshTokenResponse>.Failure(ApplicationErrors.InvalidRefreshToken);
        }

        if (existingRefreshToken.User is null)
        {
            return TResult<RefreshTokenResponse>.Failure(ApplicationErrors.NotFound);
        }

        if (await _userManager.IsLockedOutAsync(existingRefreshToken.User))
        {
            return TResult<RefreshTokenResponse>.Failure(ApplicationErrors.UserLockedOut);
        }

        var newRefreshToken = Domain.Entities.RefreshToken.Create(existingRefreshToken.UserId!);

        using var transaction = _authUnitOfWork.BeginTransaction();
        existingRefreshToken.Revoke();
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);
        transaction.Commit();

        var tenant = await _tenantRepository.GetByIdAsync(existingRefreshToken.User.TenantId, cancellationToken);
        if (tenant is null)
        {
            return TResult<RefreshTokenResponse>.Failure(ApplicationErrors.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(existingRefreshToken.User);
        var newAccessToken = _jwtTokenService.GenerateToken(existingRefreshToken.User, tenant.PublicId, roles);

        return TResult<RefreshTokenResponse>.Success(new RefreshTokenResponse(newAccessToken, newRefreshToken.Token!));
    }
}
