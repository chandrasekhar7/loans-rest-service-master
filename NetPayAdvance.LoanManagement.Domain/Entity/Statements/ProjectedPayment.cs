using System;
using NetPayAdvance.LoanManagement.Domain.Entity;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Statements
{
    public class ProjectedPayment : DomainEntity
    {
        public int LoanID { get; }

        public DateOnly OrigDueDate { get; }
        
        public Period Period { get; }
        
        public decimal RemainingPrincipal { get; }
        
        public Amount Amount { get; }
        
        public decimal Payment { get; }

        public bool Skipped { get; internal set; }
        
        private ProjectedPayment() { }

        public ProjectedPayment(int loanId, Amount amount, decimal payment, decimal remainingPrincipal, 
            Period period, DateOnly origDueDate, bool skipped = false)
        {
            LoanID = loanId;
            Amount = amount;
            Payment = payment;
            RemainingPrincipal = remainingPrincipal;
            Period = period;
            OrigDueDate = origDueDate;
            Skipped = skipped;
        }
    }
}
