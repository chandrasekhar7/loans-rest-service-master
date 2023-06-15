using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Adjustments
{
    public class StatementAdjustment : DomainEntity
    {
        public StatementId StatementId => new(LoanID, OrigDueDate);
        public int LoanID { get; }
        public int AdjustmentID { get; }
        public int LoanAdjustmentID { get; }
        public DateOnly OrigDueDate { get; }
        public Adjustment Adjustment { get; }
        public string Teller { get;  }

        private StatementAdjustment() { }

        public StatementAdjustment(AdjustmentType adjCode, StatementId statementId, Amount amount,
            string teller)
        {
            LoanID = statementId.LoanId;
            Adjustment = new Adjustment(adjCode, amount);
            OrigDueDate = statementId.OrigDueDate;
            Teller = teller;
        }

        public StatementAdjustment(Credit adjCode, StatementId statementId, Amount amount, string teller) :
            this((AdjustmentType) adjCode, statementId, amount, teller)
        {
        }

        public StatementAdjustment(Debit adjCode, StatementId statementId, Amount amount, string teller) :
            this((AdjustmentType) adjCode, statementId, amount, teller)
        {
        }
    }
}