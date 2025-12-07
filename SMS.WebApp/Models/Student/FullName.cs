using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models.Student
{
    public class FullName
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;
    }
}
