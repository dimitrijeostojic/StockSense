using FluentValidation;

namespace Application.AuthManagement.ChangePassword;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.CurrentPassword)
                  .MinimumLength(6);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.");

        RuleFor(x => x.NewPassword)
                  .MinimumLength(6);

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm new password is required.");

        RuleFor(x => x.ConfirmNewPassword)
                  .MinimumLength(6);

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword).WithMessage("Confirm new password must match the new password.");
    }
}