using FluentValidation;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.UpdateAcademicYear;

namespace SMS.Accademic.AcademicYears.Core.DeleteAcademicYear.Commands.UpdateStudent
{
    public class UpdateAcademicYearCommandValidator : AbstractValidator<UpdateAcademicYearCommand>
    {
        // Define a constant for time conversion to ensure reusability
        private static readonly TimeOnly Midnight = new TimeOnly(0, 0);

        // Helper method to calculate TotalDays difference between two DateOnly objects
        private static double GetDateDifferenceInDays(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate == null || endDate == null)
                return 0;

            // Convert to DateTime using a fixed TimeOnly (Midnight) for accurate day difference
            var startDateTime = startDate.Value.ToDateTime(Midnight);
            var endDateTime = endDate.Value.ToDateTime(Midnight);

            // FIX: Ensure TotalDays is spelled correctly and is used as a property.
            return (endDateTime - startDateTime).TotalDays;
        }

        public UpdateAcademicYearCommandValidator()
        {
            // 1. ID Validation (Essential)
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The Student ID is required for updating the record.");

            // --- 1. 🏷️ Academic Year Name Validation ---
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("The Academic Year Name is required.")
                .MaximumLength(20)
                    .WithMessage("The Academic Year Name must not exceed 20 characters.")
                // Custom Rule: Name must match the expected 'YYYY-YYYY' format from the dates
                .Must((dto, name) => name.Equals(
                    $"{dto.StartDate?.Year}-{dto.EndDate?.Year}",
                    StringComparison.Ordinal))
                    .WithMessage(dto => $"The Name must match the date format: {dto.StartDate?.Year}-{dto.EndDate?.Year}.");

            // --- 2. 🗓️ Date Range Validation: Start Date ---
            RuleFor(x => x.StartDate)
                .NotEmpty()
                    .WithMessage("The Start Date is required.")
                // Business Rule: Start Date cannot be in the past (unless for historical/current entry)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
                    .When(x => !x.IsCurrent)
                    .WithMessage("The Start Date cannot be in the past.");

            // --- 3. 📅 Date Range Validation: End Date (All EndDate rules chained here) ---
            RuleFor(x => x.EndDate)
                .NotEmpty()
                    .WithMessage("The End Date is required.")
                // Business Rule: EndDate must be strictly after StartDate
                .GreaterThan(x => x.StartDate)
                    .WithMessage("The End Date must be after the Start Date.")

                // Business Rule: Minimum Duration (at least 1 year / 365 days)
                .Must((dto, endDate) => GetDateDifferenceInDays(dto.StartDate, endDate) >= 360)
                    .WithMessage("The academic period must be at least one year (360 days) long.")

                // Business Rule: Maximum Duration (less than or equal to 2 years / 700 days)
                .Must((dto, endDate) => GetDateDifferenceInDays(dto.StartDate, endDate) <= 700)
                    .WithMessage("The academic period duration cannot exceed 2 years.");
        }
    }
}
