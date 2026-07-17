using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.Login;

internal sealed class LoginRequestHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    ITenantRepository tenantRepository)
    : IRequestHandler<LoginRequest, TResult<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtTokenService _jwtRepository = jwtRepository ?? throw new ArgumentNullException(nameof(jwtRepository));
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IAuthUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));

    public async Task<TResult<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return TResult<LoginResponse>.Failure(ApplicationErrors.InvalidCredentials);
        }

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant is null)
        {
            return TResult<LoginResponse>.Failure(ApplicationErrors.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtRepository.GenerateToken(user, tenant.PublicId, tenant.Name, roles);
        var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<LoginResponse>.Success(new LoginResponse
        (accessToken, refreshToken.Token));
    }
}
