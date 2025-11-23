namespace SMS.Domain.Exceptions
{
    namespace SMS.Domain.Exceptions
    {
        public class ClassCapacityExceededException : Exception
        {
            public int ClassId { get; }
            public int CurrentCapacity { get; }
            public int MaxCapacity { get; }

            public ClassCapacityExceededException(int classId, int currentCapacity, int maxCapacity)
                : base($"Enrollment failed for Class ID {classId}. Current enrollment ({currentCapacity}) has reached maximum capacity ({maxCapacity}).")
            {
                ClassId = classId;
                CurrentCapacity = currentCapacity;
                MaxCapacity = maxCapacity;
            }

            public ClassCapacityExceededException(string message)
                : base(message) { }

            public ClassCapacityExceededException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
