using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Application.Models.Loans;

public class AdjustmentModel
{
    public int AdjustmentId { get; set; }
    public int LoanId { get; set; }
    public int? PaymentId { get; set; }
    public int AdjustmentCodeId { get; set; }
    public DateTime CreatedOn { get; set; }
    public Amount Amount { get; set; }
    public string Teller { get; set; }
    public List<StatementAdjustmentModel> StatementAdjustments { get; set; }

    public AdjustmentModel(LoanAdjustment la)
    {
        AdjustmentId = la.AdjustmentID;
        LoanId = la.LoanID;
        PaymentId = la.PaymentID;
        AdjustmentCodeId = la.Adjustment.AdjustmentCodeID;
        CreatedOn = la.Adjustment.CreatedOn;
        Amount = la.Adjustment.Amount;
        Teller = la.Teller;
        StatementAdjustments = la.StatementAdjustments.Select(s => new StatementAdjustmentModel(s)).ToList();
    }
}