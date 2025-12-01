namespace SMS.Application.Exceptions
{
    /// <summary>
    /// Represents an error due to invalid business logic or an unacceptable state (e.g., non-unique data).
    /// This exception is typically caught globally and translated into an HTTP 400 Bad Request response.
    /// </summary>
    public class BadRequestException : Exception
    {
        // 1. Default Constructor
        public BadRequestException()
            : base("Bad request due to an unfulfilled business requirement.")
        {
        }

        // 2. Constructor accepting a custom error message
        public BadRequestException(string message)
            : base(message)
        {
        }

        // 3. Constructor accepting a message and an inner exception (for chaining)
        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}