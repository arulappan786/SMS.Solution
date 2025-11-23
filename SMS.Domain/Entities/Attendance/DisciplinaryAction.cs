using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Attendance
{
    public class DisciplinaryAction : BaseEntity
    {
        // Foreign Keys (FKs)
        public int StudentId { get; set; }
        public int TeacherId { get; set; } // The staff member who reported/issued the action

        // Properties
        public DateTime Date { get; set; }
        public string? IncidentDetails { get; set; } // Detailed description of the incident
        public string? Reason { get; set; } // Category of the offense (e.g., "Tardiness", "Bullying", "Vandalism")
        public string? Severity { get; set; } // e.g., "Minor", "Moderate", "Severe"
        public string? ActionTaken { get; set; } // The consequence (e.g., "Warning", "Detention", "Suspension")

        // Navigation Properties
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
