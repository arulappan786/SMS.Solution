using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Enums;

namespace SMS.Domain.Entities.Grading
{
    public class Assignment : BaseEntity
    {
        // Foreign Key (FK) to link to the specific class/subject combination
        public Guid ClassSubjectId { get; set; }

        // Properties
        public required string Title { get; set; } // e.g., "Chapter 3 Quiz", "Final Project"
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }
        public AssignmentType AssignmentType { get; set; } // e.g., "Homework", "Quiz", "Project", "Midterm"
        public decimal WeightPercentage { get; set; } // Weight toward the final subject grade

        // Navigation Properties
        public ClassSubject? ClassSubject { get; set; }
        public ICollection<Grade> Grades { get; set; } = new List<Grade>(); // Individual student scores
    }
}
