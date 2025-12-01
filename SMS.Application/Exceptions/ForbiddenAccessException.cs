namespace SMS.Application.Exceptions
{
    /// <summary>
    /// Represents an error where a user is authenticated but is not authorized 
    /// to perform the requested operation (HTTP 403 Forbidden).
    /// </summary>
    public class ForbiddenAccessException : Exception
    {
        // 1. Default Constructor
        public ForbiddenAccessException()
            : base("Access to the requested resource is forbidden.")
        {
        }

        // 2. Constructor accepting a custom error message
        public ForbiddenAccessException(string message)
            : base(message)
        {
        }

        // 3. Constructor accepting a message and an inner exception (for chaining)
        public ForbiddenAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}