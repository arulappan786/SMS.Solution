using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Enums;

namespace SMS.Domain.Entities.Attendance
{
    public class Attendance : BaseEntity
    {
        // Foreign Keys (FKs)
        public Guid StudentId { get; set; }
        // Optional: Use ClassSubjectId to track attendance per scheduled class (more granular)
        public Guid? ClassSubjectId { get; set; }
        // If ClassSubjectId is null, this attendance is recorded for the whole day

        // Properties
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; } // e.g., "Present", "Absent", "Late", "Excused"
        public string? Remarks { get; set; } // Reason for absence or lateness

        // Navigation Properties
        public Student? Student { get; set; }
        public ClassSubject? ClassSubject { get; set; }
    }
}
