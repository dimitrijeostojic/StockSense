using Domain.Core;
using MediatR;

namespace Application.AuthManagement.ForgotPassword;

public sealed class ForgotPasswordRequest
    : IRequest<Result>
{
    public required string Email { get; set; }
}
