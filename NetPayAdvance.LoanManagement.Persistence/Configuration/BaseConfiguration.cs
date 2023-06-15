using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : DomainEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
