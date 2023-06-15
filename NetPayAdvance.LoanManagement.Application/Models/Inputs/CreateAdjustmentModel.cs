using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Application.Models.Inputs
{
    public class CreateAdjustmentModel
    {
        public AdjustmentType AdjustmentType { get; set; }
        
        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal CabFees { get; set; }

        public decimal NSF { get; set; }

        public decimal LateFees { get; set; }

        public string Notes { get; set; }
    }
}
