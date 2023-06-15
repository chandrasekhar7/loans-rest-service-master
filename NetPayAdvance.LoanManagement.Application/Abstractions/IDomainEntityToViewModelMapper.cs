using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Abstractions
{
    public interface IDomainEntityToViewModelMapper<TDomainEntity, TViewModel> where TDomainEntity : IDomainEntity where TViewModel : IViewModel
    {
        TViewModel Map(TDomainEntity source);

        void Map(TDomainEntity source, TViewModel destination);
    }
}
