using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Models.BillingStatements;

public class BillingStatementCreateModel
{
    public StatementId StatementId { get; }

    public BillingStatementCreateModel(StatementId statementId)
    {
        StatementId = statementId;
    }
}