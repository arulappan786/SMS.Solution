namespace SMS.Application.DTOs.Accademic.Classes
{
    public record ClassesDto(Guid Id, string Name, int MaxCapacity, Guid AcademicYearId, string AcademicYearName);
}
