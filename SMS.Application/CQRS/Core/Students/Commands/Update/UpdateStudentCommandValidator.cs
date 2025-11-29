using FluentValidation;

namespace SMS.Application.CQRS.Core.Students.Commands.Update
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator()
        {
            // 1. ID Validation (Essential)
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The Student ID is required for updating the record.");

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

            // 3. Date of Birth Validation
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of Birth is required.")
                // Business Rule: Student must be at least 3 years old (example)
                .Must(date => date <= DateOnly.FromDateTime(DateTime.Today.AddYears(-3)))
                .WithMessage("Student must be at least 3 years old.");

            // 4. Gender
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            // 5. HomeAddress (Assuming Address is a Value Object)
            RuleFor(x => x.HomeAddress).NotNull().WithMessage("Home address details are required.");

            // Example of a custom rule for address validation:
            RuleFor(x => x.HomeAddress.City)
                .NotEmpty().WithMessage("City is required.");
        }
    }
}
