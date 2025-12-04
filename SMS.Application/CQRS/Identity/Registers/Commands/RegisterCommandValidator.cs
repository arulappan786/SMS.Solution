using FluentValidation;

namespace SMS.Application.CQRS.Identity.Registers.Commands
{
    // The validator must inherit from AbstractValidator<T> where T is your command/DTO
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            // --- 1. User Name Validation ---
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User Name is required.")
                .Length(3, 50).WithMessage("User Name must be between 3 and 50 characters.");

            // --- 2. Email Validation ---
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            // --- 3. Password Validation ---
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            // --- 4. Password Confirmation Check ---
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");
        }
    }
}
