using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Grading
{
    public class Grade : BaseEntity
    {
        // Foreign Keys (FKs)
        public Guid StudentId { get; set; }
        public Guid AssignmentId { get; set; }

        // Properties
        public decimal MarksObtained { get; set; }
        public string? GradeLetter { get; set; } // e.g., "A", "B+", "C" (Calculated based on MarksObtained)
        public DateTime DateRecorded { get; set; }

        // Navigation Properties
        public Student? Student { get; set; }
        public Assignment? Assignment { get; set; }
    }
}
