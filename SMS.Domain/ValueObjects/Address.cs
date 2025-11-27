namespace SMS.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }

        /// <summary>
        /// Private constructor for ORM (like EF Core) materialization only.
        /// It must be parameterless and empty to avoid running validation during object hydration.
        /// </summary>
        private Address() { } // FIX: Removed the 'this(...)' chain.

        /// <summary>
        /// Provides a structurally valid, empty placeholder address for domain initialization.
        /// Uses 'N/A' to satisfy the 'string.IsNullOrWhiteSpace' check.
        /// </summary>
        public static Address Empty
        {
            get
            {
                // FIX: Uses "N/A" placeholders to pass validation in the public constructor.
                return new Address("N/A", "N/A", "N/A", "N/A", "N/A");
            }
        }

        /// <summary>
        /// Public constructor for creating new Address Value Objects in the domain.
        /// Contains core domain validation logic.
        /// </summary>
        public Address(string street, string city, string state, string zipCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street address cannot be empty.", nameof(street));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));

            // Basic validation
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }

        // Must override GetEqualityComponents to enable structural equality comparison
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return ZipCode;
            yield return Country;
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {State} {ZipCode}, {Country}";
        }
    }
}