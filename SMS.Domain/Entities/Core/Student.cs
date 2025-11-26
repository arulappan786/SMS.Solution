using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Entities.Grading;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

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

        // Private constructor required by Entity Framework Core
        private Student() 
        {
            HomeAddress = Address.Empty;
            FullName = FullName.Empty;
            StudentCode = string.Empty; // Set required string properties to empty

        }

        // Public constructor for creating a new Student entity in the domain
        [SetsRequiredMembers]
        public Student(
            Guid? userId,
            FullName fullName,
            Address homeAddress,
            DateTime dateOfBirth,
            Gender gender,
            string email,
            string studentCode,
            DateTime enrollmentDate)
        {
            //// Enforce invariants (basic checks)
            //if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID must be valid.", nameof(userId));
            //if (string.IsNullOrWhiteSpace(studentCode)) throw new ArgumentException("Student code is required.", nameof(studentCode));

            UserId = userId;
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            HomeAddress = homeAddress ?? throw new ArgumentNullException(nameof(homeAddress));
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
