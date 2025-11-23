namespace SMS.Domain.Exceptions
{
    namespace SMS.Domain.Exceptions
    {
        public class EnrollmentClosedException : Exception
        {
            public DateTime EnrollmentClosedDate { get; }

            public EnrollmentClosedException(DateTime closedDate)
                : base($"Enrollment is currently closed. The deadline was {closedDate:yyyy-MM-dd}.")
            {
                EnrollmentClosedDate = closedDate;
            }

            public EnrollmentClosedException(string message)
                : base(message) { }

            public EnrollmentClosedException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
