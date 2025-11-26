using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Academic
{
    public class Class : BaseEntity
    {
        // Properties
        public required string Name { get; set; } // e.g., "Grade 10 - Section A"
        public int MaxCapacity { get; set; }

        // Foreign Key (FK) to AcademicYear
        public Guid AcademicYearId { get; set; }

        // Navigation Property
        public AcademicYear? AcademicYear { get; set; }

        // Navigation Properties
        public ICollection<Student> Students { get; set; } = new List<Student>(); // Students enrolled in this class
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>(); // Subjects taught in this class
    }
}
