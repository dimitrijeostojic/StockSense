using Domain.Core;
using MediatR;

namespace Application.AuthManagement.ResetPassword;

public sealed class ResetPasswordRequest
    : IRequest<Result>
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}
