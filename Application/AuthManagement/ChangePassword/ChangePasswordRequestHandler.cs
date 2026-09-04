using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.ChangePassword;

internal sealed class ChangePasswordRequestHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<ChangePasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<Result> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_currentUserAccessor.UserId);
        if (user == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        var identityResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            var errorMessage = string.Join(" ", identityResult.Errors.Select(e => e.Description));
            return Result.Failure(new Error("ChangePassword.Failed", errorMessage));
        }
        return Result.Success();
    }
}
