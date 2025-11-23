using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Enums;

namespace SMS.Domain.Entities.Scheduling
{
    public class Room : BaseEntity
    {
        // Properties
        public required string Name { get; set; } // e.g., "Science Lab 101", "Room 3B"
        public int Capacity { get; set; }
        public RoomType RoomType { get; set; } // e.g., "Classroom", "Laboratory", "Auditorium"

        // Navigation Property (Inverse of ClassSchedule.Room)
        public ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
    }
}
