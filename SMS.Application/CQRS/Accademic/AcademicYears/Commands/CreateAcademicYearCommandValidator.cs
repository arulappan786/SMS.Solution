using FluentValidation;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands
{
    public class CreateAcademicYearCommandValidator : AbstractValidator<CreateAcademicYearCommand>
    {
        public CreateAcademicYearCommandValidator()
        {
            // --- 1. Name Validation ---

            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("The Academic Year Name is required.")
                .MaximumLength(20)
                    .WithMessage("The Academic Year Name must not exceed 20 characters.")
                // Custom Rule: The Name must be derived from StartDate (YYYY) and EndDate (YYYY)
                .Must((dto, name) =>
                {
                    // Calculate expected name: Start year to End year
                    string expectedName = $"{dto.StartDate.Year}-{dto.EndDate.Year}";
                    return name.Equals(expectedName, StringComparison.Ordinal);
                })
                    .WithMessage(dto => $"The Academic Year Name '{dto.Name}' must match the format derived from the dates: {dto.StartDate.Year}-{dto.EndDate.Year}.");


            //RuleFor(x => x.Name)
            //    .NotEmpty()
            //        .WithMessage("The Academic Year Name is required.")
            //    .MaximumLength(20)
            //        .WithMessage("The Academic Year Name must not exceed 20 characters.")
            //    // Example of a format validation (e.g., must be YYYY-YYYY)
            //    .Matches(@"^\d{4}-\d{4}$")
            //        .WithMessage("The Academic Year Name must be in the format 'YYYY-YYYY', e.g., 2024-2025.");

            // --- 2. Date Validation (Sequencing and Time) ---

            RuleFor(x => x.StartDate)
                .NotEmpty()
                    .WithMessage("The Start Date is required.")
                // Business Rule: Start Date should not be in the past (unless for historical data entry)
                .Must(date => date.Date >= DateTime.Today.Date)
                    .When(x => !x.IsCurrent) // Only enforce for non-historical/non-current entry if necessary
                    .WithMessage("The Start Date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                    .WithMessage("The End Date is required.")
                // Business Rule: EndDate must be strictly after StartDate
                .GreaterThan(x => x.StartDate)
                    .WithMessage("The End Date must be after the Start Date.");

            // Business Rule: Ensure the period is reasonable (e.g., must be less than 2 years)
            RuleFor(x => x.EndDate)
                .Must((dto, endDate) => (endDate - dto.StartDate).TotalDays <= 730) // Max 2 years
                    .WithMessage("The academic period duration cannot exceed 2 years.");

            // --- 3. IsCurrent Flag Validation ---

            // While simple boolean checks are often sufficient, here is an example of a 
            // business rule validation that might require further context from the database:

            // RuleFor(x => x.IsCurrent)
            //     .Must((dto, isCurrent) => 
            //     {
            //         // HYPOTHETICAL RULE: If the user sets IsCurrent to true, 
            //         // we must ensure no other active year in the DB has IsCurrent = true.
            //         // This check usually requires injecting a Repository/Service into the validator 
            //         // or handling the uniqueness check in the Command Handler.
            //         return true; // Simple boolean validation, no internal checks needed here
            //     });
        }
    }
}
