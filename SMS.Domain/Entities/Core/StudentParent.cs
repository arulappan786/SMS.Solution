using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Enums;

namespace SMS.Domain.Entities.Core
{
    public class StudentParent : BaseEntity
    {
        // Foreign Keys (FKs)
        public int StudentId { get; set; }
        public int ParentId { get; set; }

        // Relationship Detail (Optional)
        public RelationshipToStudent RelationshipToStudent { get; set; } // e.g., "Mother", "Father", "Guardian"

        // Navigation Properties
        public Student? Student { get; set; }
        public Parent? Parent { get; set; }
    }
}
