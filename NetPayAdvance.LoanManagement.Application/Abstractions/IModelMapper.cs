using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Abstractions
{
    public interface IModelMapper
    {
        // Domain Entity -> ViewModel
        TViewModel MapDomainEntityToViewModel<TDomainEntity, TViewModel>(TDomainEntity domainEntity) where TDomainEntity : IDomainEntity where TViewModel : IViewModel;

        void MapDomainEntityToViewModel<TDomainEntity, TViewModel>(TDomainEntity domainEntity, TViewModel existing) where TDomainEntity : IDomainEntity where TViewModel : IViewModel;

    }
}

