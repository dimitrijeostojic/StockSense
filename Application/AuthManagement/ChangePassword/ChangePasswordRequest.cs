using Domain.Core;
using MediatR;

namespace Application.AuthManagement.ChangePassword;

public sealed class ChangePasswordRequest : IRequest<Result>
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}
