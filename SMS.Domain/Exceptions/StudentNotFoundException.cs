namespace SMS.Domain.Exceptions
{

    namespace SMS.Domain.Exceptions
    {
        public class StudentNotFoundException : Exception
        {
            public object Identifier { get; } // Can be an int (ID) or string (Code)

            public StudentNotFoundException(object identifier)
                : base($"Student with identifier '{identifier}' was not found in the system.")
            {
                Identifier = identifier;
            }

            public StudentNotFoundException(string message)
                : base(message) { }

            public StudentNotFoundException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
