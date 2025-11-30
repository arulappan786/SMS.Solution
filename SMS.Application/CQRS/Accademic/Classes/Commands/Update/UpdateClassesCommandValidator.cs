using FluentValidation;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Update
{
    public class UpdateClassesCommandValidator : AbstractValidator<UpdateClassesCommand>
    {
        public UpdateClassesCommandValidator()
        {
            // 1. ID Validation (Essential)
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The Student ID is required for updating the record.");

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
