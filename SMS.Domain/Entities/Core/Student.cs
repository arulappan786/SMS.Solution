using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Entities.Grading;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SMS.Domain.Entities.Core
{
    public class Student : BaseEntity
    {
        // Foreign Key (FK) to link to the base identity
        public Guid? UserId { get; set; }
        public Guid? CurrentClassId { get; set; } // FK to the Class the student is currently enrolled in

        // --- Demographic Properties ---

        // Value Objects (Must be set via constructor; private set enforces immutability)
        public FullName FullName { get; private set; }
        public Address HomeAddress { get; private set; }

        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        public required string Email { get; set; }

        // --- Academic Properties ---

        public required string StudentCode { get; set; } // Unique school ID
        public DateTime EnrollmentDate { get; set; }

        // --- Constructors ---

        /// <summary>
        /// Private constructor required by Entity Framework Core for materialization. 
        /// It MUST be empty to prevent running domain initialization/validation logic during database reads.
        /// </summary>
        private Student()
        {
            // FIX: Removed all initialization. 
            // EF Core will populate FullName and HomeAddress properties from the database 
            // using the empty private constructor in the respective Value Objects.
        }

        // Public constructor for creating a new Student entity in the domain
        [SetsRequiredMembers]
        public Student(Guid? userId,
                       FullName fullName,
                       Address homeAddress,
                       DateTime dateOfBirth,
                       Gender gender,
                       string email,
                       string studentCode,
                       DateTime enrollmentDate)
        {
            // 1. Validation for UserId
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID must be a valid non-empty GUID.", nameof(userId));

            // 2. Validation for Value Objects (Null Checks)
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            HomeAddress = homeAddress ?? throw new ArgumentNullException(nameof(homeAddress));
            // NOTE: FullName and Address already have internal validation in their own constructors.

            // 3. Validation for DateOfBirth
            // Assuming a minimum age of 3 years is required for enrollment
            if (dateOfBirth == default || dateOfBirth.AddYears(3) > DateTime.Today)
                throw new ArgumentException("Date of Birth is invalid or the student is too young for enrollment (minimum 3 years old).", nameof(dateOfBirth));

            // 4. Validation for Email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            // Standard basic regex pattern for email format
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                throw new ArgumentException("Email format is invalid.", nameof(email));
            }

            // 5. Validation for StudentCode
            if (string.IsNullOrWhiteSpace(studentCode))
                throw new ArgumentException("Student Code cannot be empty.", nameof(studentCode));

            // 6. Validation for EnrollmentDate
            if (enrollmentDate.Date > DateTime.Today.Date)
                throw new ArgumentException("Enrollment Date cannot be in the future.", nameof(enrollmentDate));


            // --- Assignment if all validation passes ---

            UserId = userId;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Email = email;
            StudentCode = studentCode;
            EnrollmentDate = enrollmentDate;
        }

        // --- Navigation Properties ---

        public AppUser? AppUser { get; set; }
        public Class? CurrentClass { get; set; }
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>(); // Guardians
        public ICollection<Attendance.Attendance> AttendanceRecords { get; set; } = new List<Attendance.Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public void UpdateHomeAddress(Address newAddress)
        {
            HomeAddress = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
        }
    }
}