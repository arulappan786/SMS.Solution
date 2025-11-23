using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Enums;

namespace SMS.Domain.Entities.Finance
{
    public class FeeInvoice : BaseEntity
    {
        // Foreign Keys (FKs)
        public int StudentId { get; set; }
        public int FeeTypeId { get; set; }

        // Properties
        public required string InvoiceNumber { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AmountDue { get; set; } // The final calculated amount for this specific invoice
        public decimal AmountPaid { get; set; } = 0.00m;
        public FeeStatus Status { get; set; } // e.g., "Unpaid", "Partially Paid", "Paid", "Overdue"

        // Navigation Properties
        public Student? Student { get; set; }
        public FeeType? FeeType { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>(); // Links to payments made against this invoice
    }
}
