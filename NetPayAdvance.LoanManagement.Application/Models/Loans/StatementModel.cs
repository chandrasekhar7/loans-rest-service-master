using System;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans
{
    public class StatementModel : IViewModel
    {
        public string StatementId { get; }

        public int Extension => (DueDate.ToDateTime(TimeOnly.MinValue) - OrigDueDate.ToDateTime(TimeOnly.MinValue)).Days;
      
        public Amount Balance { get; set; }
        
        public Amount Amount { get; set; }
        
        public Period Period { get; set; }
        
        public DateOnly OrigDueDate { get; set; }
        
        public DateOnly DueDate { get; set; }


        public StatementModel()
        {
        }

        public StatementModel(Statement s)
        {
            StatementId = s.StatementId.ToString();
            Balance = s.Balance.Amount;
            Amount = s.Amount;
            Period = s.Period;
            OrigDueDate = s.OrigDueDate;
            DueDate = s.DueDate;
        }
    }
}
