using Application.Abstractions.Services;
using Application.Common.Errors;
using Domain.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserManagement.GetMyUser;

internal sealed class GetMyUserRequestHandler(
    ICurrentUserAccessor currentUserAccessor,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetMyUserRequest, TResult<GetMyUserResponse>>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<TResult<GetMyUserResponse>> Handle(GetMyUserRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserAccessor.UserId, out var userId))
        {
            return TResult<GetMyUserResponse>.Failure(ApplicationErrors.NotFound);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return TResult<GetMyUserResponse>.Failure(ApplicationErrors.NotFound);
        }
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count < 1)
        {
            return TResult<GetMyUserResponse>.Failure(ApplicationErrors.NotFound);
        }

        return TResult<GetMyUserResponse>.Success(new()
        {
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            Roles = [.. roles]
        });
    }
}
