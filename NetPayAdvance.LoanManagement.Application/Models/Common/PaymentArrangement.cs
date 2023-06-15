using System;

namespace NetPayAdvance.LoanManagement.Application.Models.Common
{
    public class PaymentArrangement
    {
        public int LoanID { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        public string CreatedBy { get; set; }
    }
}
