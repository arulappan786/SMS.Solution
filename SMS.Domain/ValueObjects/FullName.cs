namespace SMS.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        /// <summary>
        /// Private constructor for ORM (like EF Core) materialization only.
        /// It must be parameterless and empty to prevent running validation during hydration.
        /// </summary>
        private FullName() { } // FIX 1: Removed the ': this(...)' chain.

        /// <summary>
        /// Provides a structurally valid, empty placeholder for domain initialization.
        /// Uses 'N/A' or a similar placeholder to satisfy validation.
        /// </summary>
        public static FullName Empty { get; } = new FullName("N/A", "N/A"); // FIX 2: Use "N/A" placeholders.

        /// <summary>
        /// Public constructor for creating new FullName Value Objects in the domain.
        /// Contains core domain validation logic.
        /// </summary>
        public FullName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}