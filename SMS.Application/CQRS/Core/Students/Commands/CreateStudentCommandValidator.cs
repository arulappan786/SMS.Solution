using FluentValidation;

namespace SMS.Application.CQRS.Core.Students.Commands
{
    public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            // 1. FullName (Assuming FullName is a Value Object with FirstName and LastName)
            RuleFor(x => x.FullName).NotNull().WithMessage("Full name is required.");

            // Use a When clause to define rules on the inner properties only if FullName is not null
            When(x => x.FullName != null, () =>
            {
                RuleFor(x => x.FullName.FirstName)
                    .NotEmpty().WithMessage("First name cannot be empty.")
                    .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

                RuleFor(x => x.FullName.LastName)
                    .NotEmpty().WithMessage("Last name cannot be empty.")
                    .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
            });

            // 2. Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(100);

            // 3. DateOfBirth
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                // Custom rule: Must be a date in the past
                .LessThan(DateTime.Today).WithMessage("Date of birth cannot be a future date.")
                // Custom rule: Must be less than 80 years old (e.g., if this is a university student system)
                .Must(BeAValidAge).WithMessage("Student must be between 4 and 79 years old.");

            // 4. Gender
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            // 5. HomeAddress (Assuming Address is a Value Object)
            RuleFor(x => x.HomeAddress).NotNull().WithMessage("Home address details are required.");

            // Example of a custom rule for address validation:
            RuleFor(x => x.HomeAddress.City)
                .NotEmpty().WithMessage("City is required.");
        }

        // Custom method for age validation
        private bool BeAValidAge(DateTime dob)
        {
            int age = DateTime.Today.Year - dob.Year;
            if (dob.Date > DateTime.Today.AddYears(-age)) age--;
            return age > 3 && age < 80;
        }
    }
}
