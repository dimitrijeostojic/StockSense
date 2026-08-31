using Application.Common.Interfaces;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UserManagement.GetAll;

internal sealed class GetAllUsersRequestHandler(IUserRepository userRepository, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<GetAllUsersRequest, TResult<GetAllUsersResponse>>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public async Task<TResult<GetAllUsersResponse>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync(_currentUserAccessor.TenantPublicId, cancellationToken);
        var usersDto = users.Select(user => new GetAllUsersDto
        {
            UserPublicId = Guid.Parse(user.Id),
            FirstName = user.FirstName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Username = user.UserName ?? string.Empty
        }).ToList();
        var response = new GetAllUsersResponse(usersDto);
        return TResult<GetAllUsersResponse>.Success(response);
    }
}
