using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Academic
{
    public class Subject : BaseEntity
    {
        // Properties
        public required string Name { get; set; } // e.g., "Mathematics", "Physics"
        public required string Code { get; set; } // e.g., "MTH101"

        // Navigation Property (Many-to-Many through ClassSubject)
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
