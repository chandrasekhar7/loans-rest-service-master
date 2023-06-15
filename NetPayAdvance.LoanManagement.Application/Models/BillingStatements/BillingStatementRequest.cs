
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using System;

namespace NetPayAdvance.LoanManagement.Application.Models.BillingStatements
{
    public class BillingStatementRequest
    {
        public DateOnly OrigDueDate { get; set; }

        public DateOnly DueDate { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public AdjustmentAggregate AdjAg { get; set; }

        private BillingStatementRequest() { }

        public BillingStatementRequest(DateOnly origDueDate, DateOnly dueDate, DateOnly startDate, DateOnly endDate, AdjustmentAggregate adjAg)
        {
            OrigDueDate = origDueDate;
            DueDate = dueDate;
            StartDate = startDate;
            EndDate = endDate;
            AdjAg = adjAg;
        }

    }
}
