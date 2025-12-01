using FluentValidation.Results; // Assuming you use FluentValidation

namespace SMS.Application.Exceptions
{
    public class ValidationException : Exception
    {
        // 1. Add the public property that the filter is trying to access.
        // The type should match what ValidationProblemDetails expects 
        // (usually a Dictionary<string, string[]> or IEnumerable<ValidationFailure>).
        public IDictionary<string, string[]> Errors { get; }

        // 2. Constructor to initialize the Errors property
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base("One or more validation failures have occurred.")
        {
            // Group the FluentValidation failures by property name and store the errors.
            Errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    fg => fg.Key,
                    fg => fg.Select(f => f.ErrorMessage).ToArray()
                );
        }

        // Add a parameterless constructor for general use if needed
        public ValidationException() : this(new List<ValidationFailure>()) { }
    }
}