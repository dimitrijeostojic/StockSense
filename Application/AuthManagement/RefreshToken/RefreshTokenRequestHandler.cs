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
    IJwtTokenService jwtTokenService
    )
    : IRequestHandler<RefreshTokenRequest, TResult<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));

    public async Task<TResult<RefreshTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingRefreshToken is null || existingRefreshToken.IsRevoked || existingRefreshToken.ExpiresOnUtc <= DateTime.UtcNow)
        {
            return TResult<RefreshTokenResponse>.Failure(ApplicationErrors.InvalidRefreshToken);
        }

        // Token rotation — revoke old, issue new
        existingRefreshToken = existingRefreshToken.Revoke();
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);
        var newRefreshToken = Domain.Entities.RefreshToken.Create(existingRefreshToken.UserId!);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(existingRefreshToken.User!);
        var newAccessToken = _jwtTokenService.GenerateToken(existingRefreshToken.User!, roles);

        return TResult<RefreshTokenResponse>.Success(new RefreshTokenResponse(newAccessToken, newRefreshToken.Token!));
    }
}
