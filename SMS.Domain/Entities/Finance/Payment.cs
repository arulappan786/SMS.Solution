using SMS.Domain.Entities.EntityBase;
using SMS.Domain.Enums;

namespace SMS.Domain.Entities.Finance
{
    public class Payment : BaseEntity
    {
        // Foreign Key (FK)
        public int InvoiceId { get; set; } // Links the payment to the specific invoice it is settling

        // Properties
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; } // e.g., "Cash", "Credit Card", "Bank Transfer"
        public required string TransactionReference { get; set; } // Unique ID from the payment gateway/bank

        // Navigation Property
        public FeeInvoice? FeeInvoice { get; set; }
    }
}
