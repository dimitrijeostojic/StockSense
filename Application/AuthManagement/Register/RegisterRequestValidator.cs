using FluentValidation;

namespace Application.AuthManagement.Register;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.PIB)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(255);
    }
}