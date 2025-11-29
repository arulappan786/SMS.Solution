using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Academic
{
    public class AcademicYear : BaseEntity
    {
        public required string Name { get; set; } // e.g., "2024-2025"
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrent { get; set; } // True if this is the active year

        // Navigation Property (Inverse of Class.AcademicYear)
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
