namespace SMS.Application.DTOs.Accademic.AcademicYears
{
    public class UpdateAcademicYearDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } // e.g., "2024-2025"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; } // True if this is the active year
    }
}
