using System;

namespace NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;

public class SkipChange
{
    public DateOnly OrigDueDate { get; }
    public string CreatedBy { get; }

    private SkipChange() { }

    public SkipChange(DateOnly origDueDate, string createdBy)
    {
        OrigDueDate = origDueDate;
        CreatedBy = createdBy;
    }
}