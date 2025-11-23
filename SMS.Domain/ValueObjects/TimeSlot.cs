namespace SMS.Domain.ValueObjects
{
    public class TimeSlot : ValueObject
    {
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        private TimeSlot() { }

        public TimeSlot(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("StartTime must be strictly before EndTime for a TimeSlot.");
            }

            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan Duration => EndTime - StartTime;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartTime;
            yield return EndTime;
        }
    }
}