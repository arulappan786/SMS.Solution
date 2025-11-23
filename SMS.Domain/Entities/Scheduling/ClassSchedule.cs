using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Scheduling
{
    public class ClassSchedule : BaseEntity
    {
        // Foreign Keys (FKs)
        public int ClassSubjectId { get; set; } // Links to the Class and Subject being taught
        public int RoomId { get; set; }

        // Properties
        public DayOfWeek DayOfWeek { get; set; } // Use the built-in C# Enum
        public TimeSpan StartTime { get; set; } // Time when the class starts
        public TimeSpan EndTime { get; set; } // Time when the class ends

        // Navigation Properties
        public ClassSubject? ClassSubject { get; set; }
        public Room? Room { get; set; }
    }

    
}
