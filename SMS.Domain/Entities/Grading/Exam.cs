using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Grading
{
    public class Exam : BaseEntity
    {
        // Foreign Key (FK)
        public Guid AcademicYearId { get; set; }

        // Properties
        public required string Name { get; set; } // e.g., "First Semester Final Exam", "Annual Comprehensive Test"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPublished { get; set; } = false;

        // Navigation Properties
        public AcademicYear? AcademicYear { get; set; }
        public ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
    }
}
