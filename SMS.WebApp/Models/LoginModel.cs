using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models
{
    public class LoginModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

    }
}
