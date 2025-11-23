using SMS.Domain.Entities.EntityBase;

namespace SMS.Domain.Entities.Finance
{
    public class FeeType : BaseEntity
    {
        // Properties
        public required string Name { get; set; } // e.g., "Annual Tuition", "Technology Fee", "Library Fee"
        public decimal BaseAmount { get; set; } // Default or base charge for this fee type
        public bool IsMandatory { get; set; } // Is this fee required for all students?

        // Navigation Property (Inverse of FeeInvoice.FeeType)
        public ICollection<FeeInvoice> FeeInvoices { get; set; } = new List<FeeInvoice>();
    }
}
