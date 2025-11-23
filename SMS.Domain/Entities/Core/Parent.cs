using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Entities.Identity;

namespace SMS.Domain.Entities.Core
{
    public class Parent : BaseEntity
    {
        // Foreign Key (FK)
        public int UserId { get; set; }

        // Properties
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public required string PrimaryPhone { get; set; }
        public string? Occupation { get; set; }

        // Navigation Properties
        public AppUser? User { get; set; }
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>(); // Links to their children
    }
}
