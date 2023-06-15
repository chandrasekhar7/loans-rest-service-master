using Mapster;

namespace NetPayAdvance.LoanManagement.Application.Mapster
{
    public abstract class BaseMapper { }
    
    public abstract class BaseMapper<TSource, TDestination> : BaseMapper where TSource : notnull where TDestination : notnull
    {
        protected TypeAdapterConfig Config { get; init; } = new TypeAdapterConfig();

        protected BaseMapper()
        {
            Configure(Config.NewConfig<TSource, TDestination>());
            PerformAdditionalInitialization();
        }

        public virtual TDestination Map(TSource source) => source.Adapt<TDestination>(Config);

        public virtual void Map(TSource source, TDestination destination) => source.Adapt(destination, typeof(TSource), typeof(TDestination), Config);

        protected virtual TypeAdapterSetter<TSource, TDestination> Configure(TypeAdapterSetter<TSource, TDestination> typeAdapterSetter) => typeAdapterSetter;

        protected virtual void PerformAdditionalInitialization() { }
    }
}

