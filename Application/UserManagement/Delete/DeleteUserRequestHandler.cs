using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Constants;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserManagement.Delete;

internal sealed class DeleteUserRequestHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor,
    UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteUserRequest, Result>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
