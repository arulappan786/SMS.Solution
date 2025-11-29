using FluentValidation;

namespace SMS.Application.CQRS.Core.Students.Commands.DeleteStudent
{
    public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
    {
        public DeleteStudentCommandValidator()
        {
            // The only rule needed is to ensure the ID is not empty
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The Student ID is required to delete the record.");
        }
    }
}
