using Domain.Core;
using MediatR;

namespace Application.AuthManagement.Login;

public sealed record LoginRequest(string Email, string Password)
    : IRequest<TResult<LoginResponse>>;
