using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Mapster
{
    public class ModelMapper : IModelMapper
    {
        private readonly IServiceProvider provider;

        private readonly ConcurrentDictionary<Type, object> lookup = new ConcurrentDictionary<Type, dynamic>();

        public ModelMapper(IServiceProvider serviceProvider)
        {
            provider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TViewModel MapDomainEntityToViewModel<TDomainEntity, TViewModel>(TDomainEntity domainEntity) where TDomainEntity : IDomainEntity where TViewModel : IViewModel
            => GetFactoryMethodInstance<TDomainEntity, TViewModel>().Invoke(domainEntity);

        public void MapDomainEntityToViewModel<TDomainEntity, TViewModel>(TDomainEntity domainEntity, TViewModel existing) where TDomainEntity : IDomainEntity where TViewModel : IViewModel
            => GetVoidMethodInstance<TDomainEntity, TViewModel>().Invoke(domainEntity, existing);
        
        private Func<TSource, TTarget> GetFactoryMethodInstance<TSource, TTarget>()
        {
            var mapper = GetMapper<TSource, TTarget>();
            return (Func<TSource, TTarget>)Delegate.CreateDelegate(typeof(Func<TSource, TTarget>), mapper, "Map");
        }
        
         private object GetMapper<TSource, TTarget>()
        {
            var mapperType = (typeof(TSource), typeof(TTarget)) switch
            {
                (Type source, Type target) when typeof(IDomainEntity).IsAssignableFrom(source) && typeof(IViewModel).IsAssignableFrom(target) =>
                    typeof(IDomainEntityToViewModelMapper<,>).MakeGenericType(source, target),
                { } => throw new ApplicationLayerException($"Could not get instance of mapper class for source type {typeof(TSource).Name} and destination type {typeof(TTarget).Name}.")
            };

            return lookup.GetOrAdd(mapperType, provider.GetRequiredService(mapperType));
        }

        private Action<TSource, TTarget> GetVoidMethodInstance<TSource, TTarget>()
        {
            var mapper = GetMapper<TSource, TTarget>();
            return (Action<TSource, TTarget>)Delegate.CreateDelegate(typeof(Action<TSource, TTarget>), mapper, "Map");
        }
    }
}

