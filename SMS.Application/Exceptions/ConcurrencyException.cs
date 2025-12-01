namespace SMS.Application.Exceptions
{
    /// <summary>
    /// Represents an error where a transaction fails due to data being modified 
    /// concurrently by another user or process (HTTP 409 Conflict).
    /// </summary>
    public class ConcurrencyException : Exception
    {
        // 1. Default Constructor
        public ConcurrencyException()
            : base("The resource you were trying to update or delete has been modified by another operation.")
        {
        }

        // 2. Constructor accepting a custom error message
        public ConcurrencyException(string message)
            : base(message)
        {
        }

        // 3. Constructor accepting a message and an inner exception (for chaining)
        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Optional: Constructor to specify the Entity Type and ID for better logging/messages
        public ConcurrencyException(string entityName, object entityId)
            : base($"Concurrency failure for entity \"{entityName}\" ({entityId}).")
        {
        }
    }
}