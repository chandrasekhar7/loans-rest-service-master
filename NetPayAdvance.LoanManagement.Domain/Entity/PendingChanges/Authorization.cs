using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges
{
    public class Authorization : DomainEntity
    {
        public int LoanChangeId { get; set; }
        
        public int? CustomerChangeId { get; set; }
        
        public int LoanId { get; set; }
        
        public int? AddendumId { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public string CreatedBy { get; set; }
        
        public PendingChangeType ChangeType { get; set; }
        
        public string NewValue { get; set; }
        
        public DateTime? CancelledOn { get; set; }
        
        public string? CancelledBy { get; set; }
        
        public DateTime? CompletedOn { get; set; }
        
        public string? CompletedBy { get; set; }
        
        public void Create(StatementId statementId, PendingChangeType type, string teller)
        {
            LoanId = statementId.LoanId;
            NewValue = statementId.OrigDueDate.ToString("MM/dd/yyyy");
            ChangeType = type;
            CreatedOn = DateTime.Now;
            CreatedBy = teller;
        }

        public void Update(string teller)
        {
            CancelledBy = teller;
            CancelledOn = DateTime.Now;
        }
    }
}
