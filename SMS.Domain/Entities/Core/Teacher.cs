using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.Attendance;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Entities.Identity;

namespace SMS.Domain.Entities.Core
{
    public class Teacher : BaseEntity
    {
        // Foreign Key (FK)
        public int UserId { get; set; }

        // Properties
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime HireDate { get; set; }
        public string? Specialization { get; set; } // e.g., "High School Math"
        public required string TeacherCode { get; set; }
        public required string Email { get; set; }

        // Navigation Properties
        public AppUser? User { get; set; }
        public ICollection<ClassSubject> ClassesTaught { get; set; } = new List<ClassSubject>();
        public ICollection<DisciplinaryAction> DisciplinaryActionsIssued { get; set; } = new List<DisciplinaryAction>();
    }
}
