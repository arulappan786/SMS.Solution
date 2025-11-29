namespace SMS.Application.DTOs.Accademic.AcademicYears
{
    public class AcademicYearDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } // e.g., "2024-2025"
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrent { get; set; } // True if this is the active year
    }
}
