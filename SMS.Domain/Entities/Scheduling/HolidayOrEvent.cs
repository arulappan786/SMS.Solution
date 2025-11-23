using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Scheduling
{
    public class HolidayOrEvent : BaseEntity
    {
        // Properties
        public required string HolidayOrEventName { get; set; } // e.g., "Thanksgiving Break", "Annual Sports Day"
        public DateTime Date { get; set; }
        public required string HolidayOrEventType { get; set; } // e.g., "Holiday", "School Event", "Early Release"
        public string? Description { get; set; }
    }
}
