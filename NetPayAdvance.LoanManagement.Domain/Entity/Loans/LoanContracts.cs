using System;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans
{
    public class LoanContracts : DomainEntity
    {
        public int LoanId { get; set;  }
    
        public DateTime DateSigned { get; set; }
    }
}

