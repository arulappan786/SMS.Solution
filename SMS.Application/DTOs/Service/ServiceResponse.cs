namespace SMS.Application.DTOs.Service
{
    public class ServiceResponse
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// A human-readable message about the result (e.g., error detail or success confirmation).
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional data payload returned upon successful operation.
        /// </summary>
        public object? Data { get; set; }

        // --- Static Factory Methods ---

        /// <summary>
        /// Creates a successful ServiceResponse with an optional message and data.
        /// </summary>
        public static ServiceResponse Success(string? message = "Operation completed successfully.", object? data = null)
        {
            return new ServiceResponse
            {
                Succeeded = true,
                Message = message ?? "Operation completed successfully.",
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed ServiceResponse with a required error message.
        /// </summary>
        public static ServiceResponse Failure(string message, object? data = null)
        {
            return new ServiceResponse
            {
                Succeeded = false,
                Message = message,
                Data = data
            };
        }
    }

    /// <summary>
    /// Generic version of ServiceResponse to support specific typed data payloads.
    /// </summary>
    public class ServiceResponse<T> : ServiceResponse
    {
        public new T? Data { get; set; }

        public static ServiceResponse<T> Success(string? message = "Operation completed successfully.", T? data = default)
        {
            return new ServiceResponse<T>
            {
                Succeeded = true,
                Message = message ?? "Operation completed successfully.",
                Data = data
            };
        }

        // Note: The Failure method can simply return the non-generic ServiceResponse.Failure 
        // or be defined specifically if you want the return type to be ServiceResponse<T>.
        // For simplicity, typically only the Success method is genericized.
    }
}

