using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Models
{
    public class CreateClassModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 200)]
        public int MaxCapacity { get; set; }

        [Required]
        public Guid AcademicYearId { get; set; }
    }
}
