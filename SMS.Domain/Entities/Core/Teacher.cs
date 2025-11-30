using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.Attendance;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Entities.Identity;
using SMS.Domain.ValueObjects;

namespace SMS.Domain.Entities.Core
{
    public class Teacher : BaseEntity
    {
        // Foreign Key (FK)
        public Guid UserId { get; set; }

        // Properties
        public required FullName FullName { get; set; }
        public DateOnly HireDate { get; set; }
        public string? Specialization { get; set; } // e.g., "High School Math"
        public required string TeacherCode { get; set; }
        public required string Email { get; set; }

        // Navigation Properties
        public AppUser? User { get; set; }
        public ICollection<ClassSubject> ClassesTaught { get; set; } = new List<ClassSubject>();
        public ICollection<DisciplinaryAction> DisciplinaryActionsIssued { get; set; } = new List<DisciplinaryAction>();
    }
}
