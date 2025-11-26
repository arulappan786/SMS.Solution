using SMS.Domain.Entities.Academic;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Grading
{
    public class ExamResult : BaseEntity
    {
        // Foreign Keys (FKs)
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; } // The subject for which the score is recorded

        // Properties
        public decimal ScoreObtained { get; set; }
        public decimal MaxScore { get; set; } // The maximum score possible for this subject in this exam
        public string? Comments { get; set; }

        // Navigation Properties
        public Exam? Exam { get; set; }
        public Student? Student { get; set; }
        public Subject? Subject { get; set; }
    }
}
