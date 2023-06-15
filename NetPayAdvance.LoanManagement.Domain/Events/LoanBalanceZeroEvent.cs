using System;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Domain.Events;

public class LoanBalanceZeroEvent : IDomainEvent
{
    public int LoanId { get; }
    public DateTime OccuredAt { get; }

    public LoanBalanceZeroEvent(int loanId)
    {
        LoanId = loanId;
        OccuredAt = DateTime.Now;
    }
}