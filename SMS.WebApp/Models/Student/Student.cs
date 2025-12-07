using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models.Student
{
    public class Student
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid CurrentClassId { get; set; }

        [Required]
        public FullName FullName { get; set; } = new FullName();

        [Required]
        public Address HomeAddress { get; set; } = new Address();

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-4);

        public Gender Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
