using FluentValidation;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.DeleteAcademicYear
{
    public class DeleteAcademicYearCommandValidator : AbstractValidator<DeleteAcademicYearCommand>
    {
        public DeleteAcademicYearCommandValidator()
        {
            // The only rule needed is to ensure the ID is not empty
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The AcademicYear ID is required to delete the record.");
        }
    }
}
