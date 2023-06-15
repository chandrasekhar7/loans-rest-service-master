using System;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans
{
    public class ProjectedPaymentModel : IViewModel
    {
        public int LoanID { get; set; }
      
        public DateOnly OrigDueDate { get; set; }
        
        public Period Period { get; set; }
        
        public Amount Amount { get; set; }

        public decimal RemainingPrincipal { get; set; }
        
        public decimal Payment { get; set; }

        public ProjectedPaymentModel(ProjectedPayment p)
        {
            LoanID = p.LoanID;
            OrigDueDate = p.OrigDueDate;
            Period = p.Period;
            RemainingPrincipal = p.RemainingPrincipal;
            Payment = p.Payment;
            Amount = p.Amount;
        }
    
    }
}
