using FluentValidation;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email Address is required.")
                .EmailAddress()
                    .WithMessage("A valid Email Address is required.");
            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Password is required.");
        }
    }
}
