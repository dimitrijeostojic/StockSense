using Application.AuthManagement.RegisterManager;
using Domain.Core;
using MediatR;

namespace Application.AuthManagement.RegisterUser;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password) : IRequest<TResult<RegisterUserResponse>>;
