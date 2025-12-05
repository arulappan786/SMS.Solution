namespace SMS.WebApp.Models
{
    public class ClassesModel
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }

        public Guid AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }
    }
}
