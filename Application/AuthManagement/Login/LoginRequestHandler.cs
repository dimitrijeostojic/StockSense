using Application.Common.Errors;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.Login;

internal sealed class LoginRequestHandler(
    UserManager<ApplicationUser> userManager,
    IJwtRepository jwtRepository)
    : IRequestHandler<LoginRequest, TResult<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtRepository _jwtRepository = jwtRepository ?? throw new ArgumentNullException(nameof(jwtRepository));

    public async Task<TResult<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return TResult<LoginResponse>.Failure(ApplicationErrors.InvalidCredentials);
        }
        if (user.TwoFactorEnabled)
        {
            return TResult<LoginResponse>.Success(
                new LoginResponse(null, null, RequiresTwoFactor: true, UserId: user.Id));
        }
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _jwtRepository.GenerateTokenAsync(user, roles);
        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        return TResult<LoginResponse>.Success(new LoginResponse
        (accessToken, null, isTwoFactorEnabled, user.Id));
    }
}
