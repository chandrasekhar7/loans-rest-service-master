using System.Threading.Tasks;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent devent);
}