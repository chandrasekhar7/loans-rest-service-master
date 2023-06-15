using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Adjustments
{
    public record Adjustment
    {
        public int AdjustmentCodeID { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public Amount Amount { get; set; }

        private Adjustment() {}

        public Adjustment(AdjustmentType adjCode, Amount amount)
        {
            AdjustmentCodeID = (int)adjCode;
            CreatedOn = DateTime.Now;
            Amount = amount;
        }

        public Adjustment(Credit adjCode, Amount amount) : this((AdjustmentType) adjCode, amount)
        {
        }
        
        public Adjustment(Debit adjCode, Amount amount) : this((AdjustmentType) adjCode, amount)
        {
        }
    }
}
