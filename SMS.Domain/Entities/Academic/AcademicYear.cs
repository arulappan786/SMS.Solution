using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Academic
{
    public class AcademicYear : BaseEntity
    {
        public required string Name { get; set; } // e.g., "2024-2025"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; } // True if this is the active year

        // Navigation Property (Inverse of Class.AcademicYear)
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
