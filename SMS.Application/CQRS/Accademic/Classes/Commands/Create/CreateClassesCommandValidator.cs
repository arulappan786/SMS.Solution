using FluentValidation;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Create
{
    public class CreateClassesCommandValidator : AbstractValidator<CreateClassesCommand>
    {
        public CreateClassesCommandValidator()
        {
            // --- 1. AcademicYearId Validation ---
            RuleFor(command => command.AcademicYearId)
                .NotEmpty()
                .WithMessage("Academic Year ID is required.")
                .NotEqual(Guid.Empty)
                .WithMessage("Academic Year ID cannot be the default empty Guid.");


            // --- 2. Name Validation ---
            RuleFor(command => command.Name)
                .NotEmpty()
                .WithMessage("Class Name is required.")
                .MaximumLength(50) // Adjust the length limit as needed
                .WithMessage("Class Name cannot exceed 50 characters.");


            // --- 3. MaxCapacity Validation ---
            RuleFor(command => command.MaxCapacity)
                .NotEmpty() // Although MaxCapacity is an int, this ensures it's checked if it were a nullable type
                .WithMessage("Maximum Capacity is required.")
                .GreaterThan(0)
                .WithMessage("Maximum Capacity must be greater than zero.");
        }
    }
}
