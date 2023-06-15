using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Statements
{
    public class Statement : DomainEntity
    {
        public StatementId StatementId => new StatementId(LoanId, OrigDueDate);
        public int LoanId { get; }
        public Period Period { get; }
        public DateOnly OrigDueDate { get; }
        public DateOnly DueDate { get; private set; }
        public StatementBalance Balance { get; }
        public Amount Amount { get; set; }
        private Statement() { }
              
        public Statement(StatementId stmtId, Amount amount, Amount balance, Period period, DateOnly dueDate)
        {
            LoanId = stmtId.LoanId;
            OrigDueDate = stmtId.OrigDueDate;
            Amount = amount;
            Balance = new StatementBalance(stmtId, balance);
            Period = period ?? throw new ArgumentNullException(nameof(period));
            DueDate = dueDate;
        }

        public void ApplyAdjustment(Amount amount)
        {
            var adjusted = Balance.Amount + amount;
            if(adjusted.Principal < 0 || adjusted.Interest < 0 || adjusted.CabFees < 0 || adjusted.Nsf < 0 || adjusted.LateFees < 0)
            {
                throw new Exception("Invalid adjustment values for the statement");
            }
            Balance.Amount = adjusted;
        }

        public void ExtendDueDate(DateOnly extendDate, int maxExtension = 5)
        {
            if (extendDate < DueDate || extendDate < Period.StartDate)
            {
                throw new DomainLayerException($"Extension Date {extendDate:d} should be more than due date {DueDate:d}");
            }
            if (Balance.Amount.Total == 0)
            {
                throw new DomainLayerException("Can't extend the due date on a statement with a $0 balance");
            }
            if ((extendDate.ToDateTime(TimeOnly.MinValue) - OrigDueDate.ToDateTime(TimeOnly.MinValue)).Days > maxExtension)
            {
                throw new DomainLayerException("The maximum amount of days extended is exceed");
            }
            if (Period != null && DueDate < extendDate)
            {
                DueDate = extendDate;
            }
        }
    }
}
