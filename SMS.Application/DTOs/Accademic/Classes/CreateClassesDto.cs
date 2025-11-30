namespace SMS.Application.DTOs.Accademic.Classes
{
    public record CreateClassesDto(string Name, int MaxCapacity, Guid AcademicYearId);
}
