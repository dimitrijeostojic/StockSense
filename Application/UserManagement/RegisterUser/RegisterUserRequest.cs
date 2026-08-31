using Domain.Core;
using MediatR;

namespace Application.UserManagement.RegisterUser;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password) : IRequest<TResult<RegisterUserResponse>>;
