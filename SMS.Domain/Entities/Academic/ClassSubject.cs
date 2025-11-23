using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Entities.Grading;
using SMS.Domain.Entities.Scheduling;

namespace SMS.Domain.Entities.Academic
{
    public class ClassSubject : BaseEntity
    {

        // Foreign Keys (FKs)
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }

        // Navigation Properties
        public Class? Class { get; set; }
        public Subject? Subject { get; set; }
        public Teacher? Teacher { get; set; } // The teacher assigned to this specific subject/class combination

        // Optional: Navigation for related entities (e.g., assignments, schedules)
        public ICollection<ClassSchedule> Schedules { get; set; } = new List<ClassSchedule>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}