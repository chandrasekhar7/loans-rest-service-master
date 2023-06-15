using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration;

public class AdjustmentAggregateEntityConfiguration : BaseConfiguration<AdjustmentAggregate>
{
    public override void Configure(EntityTypeBuilder<AdjustmentAggregate> builder)
    {
        base.Configure(builder);
        builder.ToTable("Loans", "loan");
        builder.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD43778E2007B");

        builder.Property(e => e.LoanId).HasColumnName("LoanID");
        builder.HasOne(u => u.Loan).WithOne().HasForeignKey<Loan>(u => u.LoanId);
        builder.HasMany(u => u.Adjustments).WithOne().HasForeignKey(u => u.LoanID);
    }
}