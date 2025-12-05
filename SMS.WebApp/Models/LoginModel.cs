using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models
{
    public class LoginModel
    {

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }

    }
}
