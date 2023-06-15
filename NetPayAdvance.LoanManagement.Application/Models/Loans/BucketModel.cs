
namespace NetPayAdvance.LoanManagement.Application.Models.Loans
{
    public class BucketModel
    {
        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal CabFees { get; set; }

        public decimal NSF { get; set; }

        public decimal LateFees { get; set; }

    }
}
