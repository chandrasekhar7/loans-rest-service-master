using System;
using System.Collections.Generic;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Statements
{
    public class BillingStatement : DomainEntity
    {
        public StatementId StatementId => new StatementId(LoanID, OrigDueDate);
        public DateOnly OrigDueDate { get; set; }
        public int LoanID { get; set; }
        public string StatementHTML { get; set; }

        private BillingStatement() {}
        public BillingStatement(StatementId stmtId, string statementHtml)
        {
            LoanID = stmtId.LoanId;
            OrigDueDate = stmtId.OrigDueDate;
            StatementHTML = statementHtml;
        }
    }
}
