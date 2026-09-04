using Application.Common.Errors;
using Domain.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthManagement.ResetPassword;

internal sealed class ResetPasswordRequestHandler(
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Result> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("ResetPassword.Failed", errorMessage));
        }
        return Result.Success();
    }
}
