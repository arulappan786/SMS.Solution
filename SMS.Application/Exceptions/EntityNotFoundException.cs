namespace SMS.Application.Exceptions
{
    /// <summary>
    /// Custom exception for when a requested domain entity cannot be found in the persistent store.
    /// This typically translates to an HTTP 404 Not Found response in the API layer.
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        // 1. Basic constructor
        public EntityNotFoundException()
            : base("The requested entity was not found.")
        {
        }

        // 2. Constructor accepting a custom message
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        // 3. Constructor accepting message and inner exception (for chaining)
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // 4. Constructor for serialization (essential for non-web environments)
        protected EntityNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructor for easy use when only the entity name and key are known.
        /// </summary>
        public EntityNotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}