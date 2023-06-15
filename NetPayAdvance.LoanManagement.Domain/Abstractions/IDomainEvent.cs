using System;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface IDomainEvent
{
    DateTime OccuredAt { get; }
}