namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans
{
    public class LoanPermissions : DomainEntity
    {
        public int LoanId { get; set;  }
    
        public bool AutoACH { get; set; }
        
        public bool AutoDebit { get; set; }
    }

}
