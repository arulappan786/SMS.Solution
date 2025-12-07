using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models.Student
{
    public class Address
    {
        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Invalid Indian PIN Code format. Must be 6 digits and cannot start with 0.")]
        [Display(Name = "PIN Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;
    }
}
