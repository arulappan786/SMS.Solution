namespace SMS.WebApp.Models
{
    public class AcademicYearModel
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}
