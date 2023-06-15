using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _log;
    public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> log)
    {
        _mediator = mediator;
        _log = log;
    }

    public async Task Dispatch(IDomainEvent devent)
    {

        var domainEventNotification = _createDomainEventNotification(devent);
        _log.LogDebug("Dispatching Domain Event as MediatR notification.  EventType: {eventType}", devent.GetType());
        await _mediator.Publish(domainEventNotification);
    }
       
    private INotification _createDomainEventNotification(IDomainEvent domainEvent)
    {
        var genericDispatcherType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(genericDispatcherType, domainEvent);

    }
}