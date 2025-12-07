namespace SMS.WebApp.Models.Student
{
    public class StudentListItem
    {
        public Guid Id { get; set; }
        public Guid CurrentClassId { get; set; }
        public FullName FullName { get; set; } = new FullName();
        public Gender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;

        // Non-API field to hold the class name
        public string CurrentClassName { get; set; } = "N/A";
    }
}
