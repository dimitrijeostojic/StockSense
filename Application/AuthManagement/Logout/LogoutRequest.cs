using Domain.Core;
using MediatR;

namespace Application.AuthManagement.Logout;

public sealed class LogoutRequest : IRequest<Result>
{
    public required string RefreshToken { get; set; }
}
