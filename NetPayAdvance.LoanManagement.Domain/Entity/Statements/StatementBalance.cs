using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Statements;

public class StatementBalance : DomainEntity
{
    public StatementId StatementId => new StatementId(LoanId, OrigDueDate);
    public int LoanId { get; }
    public DateOnly OrigDueDate { get; }
    public Amount Amount { get; set; }

    private StatementBalance()
    {
    }

    public StatementBalance(StatementId stmtId, Amount balance)
    {
        LoanId = stmtId.LoanId;
        OrigDueDate = stmtId.OrigDueDate;
        Amount = balance;
    }
}