using Domain.Core;
using MediatR;

namespace Application.UserManagement.InviteUser;

public sealed record InviteUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Role) : IRequest<TResult<InviteUserResponse>>;
