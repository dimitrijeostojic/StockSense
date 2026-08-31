using Application.Common.Interfaces;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserManagement.GetAll;

internal sealed class GetAllUsersRequestHandler(
    IUserRepository userRepository,
    ICurrentUserAccessor currentUserAccessor,
    UserManager<ApplicationUser> userManager) : IRequestHandler<GetAllUsersRequest, TResult<GetAllUsersResponse>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<TResult<GetAllUsersResponse>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        var usersDtoTasks = users.Select(async user =>
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new GetAllUsersDto
            {
                Roles = [.. roles],
                UserPublicId = Guid.Parse(user.Id),
                FirstName = user.FirstName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Username = user.UserName ?? string.Empty
            };
        });
        var usersDto = await Task.WhenAll(usersDtoTasks);
        var response = new GetAllUsersResponse(usersDto);
        return TResult<GetAllUsersResponse>.Success(response);
    }
}
