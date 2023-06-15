using System;

namespace NetPayAdvance.LoanManagement.Application.Models.Common
{
    public class HistoryNotes
    {
        public int LoanID { get; set; }
        
        public int TransID { get; set; }
        
        public string ColumnName { get; set; }
        
        public string OldValue { get; set; }
        
        public string NewValue { get; set; }
        
        public string ChangedBy { get; set; }
        
        public DateTime ChangedOn { get; set; }
        
        public string Notes { get; set; }

        public HistoryNotes(int loanID, int transID, string column, string old, string newValue, string changedBy, string notes)
        {
            LoanID = loanID;
            TransID = transID;
            ChangedOn = DateTime.Now;
            ColumnName = column;
            NewValue = newValue;
            ChangedBy = changedBy;
            Notes = notes;
            OldValue = old;
        }
    }
}
