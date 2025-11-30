using FluentValidation;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Delete
{
    public class DeleteClassesCommandValidator : AbstractValidator<DeleteClassesCommand>
    {
        public DeleteClassesCommandValidator()
        {
            // 1. ID Validation (Essential)
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The Class ID is required for deleting the record.");
        }
    }
}
