using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.Logout;

internal sealed class LogoutRequestHandler(
    IRefreshTokenRepository refreshTokenRepository,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutRequest, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingRefreshToken is null || existingRefreshToken.IsRevoked || existingRefreshToken.ExpiresOnUtc <= DateTime.UtcNow)
        {
            return Result.Failure(ApplicationErrors.InvalidRefreshToken);
        }

        if (existingRefreshToken.User is null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }

        if (await _userManager.IsLockedOutAsync(existingRefreshToken.User))
        {
            return Result.Failure(ApplicationErrors.UserLockedOut);
        }

        existingRefreshToken.Revoke();
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();

    }
}
