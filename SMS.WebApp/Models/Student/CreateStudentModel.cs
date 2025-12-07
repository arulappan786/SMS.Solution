using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models.Student
{
    public class CreateStudentModel
    {

        [Required]
        public Guid CurrentClassId { get; set; }

        [Required]
        public FullName FullName { get; set; } = new FullName();

        [Required]
        public Address HomeAddress { get; set; } = new Address();

        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-4));

        public Gender Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
