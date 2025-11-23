using System.Security.Cryptography;

namespace SMS.Infrastructure.Services.Common.Utilities
{
    public static class PasswordGenerator
    {
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DigitChars = "0123456789";
        // Using a common set of non-alphanumeric characters, ensuring at least one character is unique
        private const string SpecialChars = "!@#$%^&*()_-+=[{]};:<>|./?";

        // Policy constraints
        private const int RequiredLength = 8;

        // Combine all allowed characters for the rest of the password
        private const string AllChars = LowercaseChars + UppercaseChars + DigitChars + SpecialChars;

        public static string GenerateSecurePassword()
        {
            // 1. Array to hold the mandatory characters (1 Lower, 1 Upper, 1 Digit, 1 Special)
            var password = new char[RequiredLength];
            using var rng = RandomNumberGenerator.Create();

            // 2. Initialize the password with mandatory characters
            // We use a list of delegates to ensure one character from each required set is present
            var charGenerators = new List<Func<char>>
        {
            () => GetRandomChar(LowercaseChars, rng), // options.Password.RequireLowercase = true
            () => GetRandomChar(UppercaseChars, rng), // options.Password.RequireUppercase = true
            () => GetRandomChar(DigitChars, rng),     // options.Password.RequireDigit = true
            () => GetRandomChar(SpecialChars, rng)    // Ensures options.Password.RequiredUniqueChars >= 1
        };

            // 3. Randomly place the mandatory characters in the password array
            var mandatoryIndices = Enumerable.Range(0, RequiredLength).ToList();

            // Ensure the four mandatory types are placed first
            for (int i = 0; i < charGenerators.Count; i++)
            {
                // Pick a random remaining index for placement
                int indexToRemove = RandomNumberGenerator.GetInt32(0, mandatoryIndices.Count);
                int placementIndex = mandatoryIndices[indexToRemove];
                mandatoryIndices.RemoveAt(indexToRemove);

                password[placementIndex] = charGenerators[i]();
            }

            // 4. Fill the remaining spots (4 spots left in an 8-char password)
            for (int i = 0; i < RequiredLength; i++)
            {
                if (password[i] == 0) // Check for unassigned spots (value remains default char 0)
                {
                    // Fill with a character from the entire set
                    password[i] = GetRandomChar(AllChars, rng);
                }
            }

            // 5. Shuffle the array to increase randomness (optional, but good practice)
            Shuffle(password, rng);

            return new string(password);
        }

        /// <summary>
        /// Gets a random character from the provided character set.
        /// </summary>
        private static char GetRandomChar(string charSet, RandomNumberGenerator rng)
        {
            int index = RandomNumberGenerator.GetInt32(0, charSet.Length);
            return charSet[index];
        }

        /// <summary>
        /// Shuffles the array elements using the Fisher-Yates algorithm.
        /// </summary>
        private static void Shuffle(char[] array, RandomNumberGenerator rng)
        {
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}