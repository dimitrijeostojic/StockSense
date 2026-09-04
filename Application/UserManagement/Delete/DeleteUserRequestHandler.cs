using Application.Abstractions.Services;
using Application.Common.Constants;
using Application.Common.Errors;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserManagement.Delete;

internal sealed class DeleteUserRequestHandler(
    IUserRepository userRepository,
    IAuthUnitOfWork authUnitOfWork,
    ICurrentUserAccessor currentUserAccessor,
    UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteUserRequest, Result>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IAuthUnitOfWork _authUnitOfWork = authUnitOfWork ?? throw new ArgumentNullException(nameof(authUnitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    public async Task<Result> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByPublicIdAsync(request.UserPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(Roles.Admin))
        {
            return Result.Failure(ApplicationErrors.CannotDeleteAdminUser);
        }
        _userRepository.Delete(user);
        await _authUnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
