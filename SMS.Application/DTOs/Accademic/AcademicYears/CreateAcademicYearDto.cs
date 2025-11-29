namespace SMS.Application.DTOs.Accademic.AcademicYears
{
    public record CreateAcademicYearDto(string Name,
                                        DateTime StartDate,
                                        DateTime EndDate,
                                        bool IsCurrent);
}
