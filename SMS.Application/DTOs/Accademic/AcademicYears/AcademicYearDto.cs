namespace SMS.Application.DTOs.Accademic.AcademicYears
{
    public record AcademicYearDto(Guid Id,
                                  string Name,
                                  DateOnly StartDate,
                                  DateOnly EndDate,
                                  bool IsCurrent);
}