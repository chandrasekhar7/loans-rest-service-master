using System;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans;

public class StatementAdjustmentModel
{
    public string StatementId { get; set; }
    public Amount Amount { get; set; }

    public StatementAdjustmentModel(StatementAdjustment sa)
    {
        StatementId = new StatementId(sa.LoanID, sa.OrigDueDate).ToString();
        Amount = sa.Adjustment.Amount;
    }
}