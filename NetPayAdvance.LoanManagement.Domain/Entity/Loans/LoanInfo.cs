using System;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Loans;

public class LoanInfo
{
    public bool PendingAch { get; private set; }

    public LoanInfo(bool pendingAch)
    {
        PendingAch = pendingAch;
    }
}