using System.Collections.Concurrent;
using System.Collections.Generic;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Domain.Entity
{
    public abstract class DomainEntity : IDomainEntity
    {
        // private List<IDomainEvent> domainEvents = new List<IDomainEvent>();
        // public List<IDomainEvent> DomainEvents => domainEvents;
        private readonly ConcurrentQueue<IDomainEvent> domainEvents = new ConcurrentQueue<IDomainEvent>();
        public IProducerConsumerCollection<IDomainEvent> DomainEvents => domainEvents;

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            domainEvents.Enqueue(eventItem);
        }
    }
}
