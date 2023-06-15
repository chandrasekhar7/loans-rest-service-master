using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Converters;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class StatementEntityConfiguration : BaseConfiguration<Statement>
    {
        public override void Configure(EntityTypeBuilder<Statement> builder)
        {
            base.Configure(builder);
            builder.ToTable("Statements", "loan");
            builder.HasKey(e => new {e.LoanId, e.OrigDueDate });
            builder.Property(e => e.LoanId).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.Property(e => e.OrigDueDate).HasColumnType("date").HasColumnName("OrigDueDate")
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            builder.Property(e => e.DueDate).HasColumnType("date").HasColumnName("DueDate")
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            
            builder.OwnsOne(x => x.Amount, a =>
            {
                a.Property(e => e.Principal).HasColumnType("decimal").HasColumnName("Principal");
                a.Property(e => e.Interest).HasColumnType("decimal").HasColumnName("Interest");
                a.Property(e => e.CabFees).HasColumnType("decimal").HasColumnName("CabFee");
            });

            builder.OwnsOne(x => x.Period, a =>
            {
                a.Property(e => e.StartDate).HasColumnType("date").HasColumnName("StartDate")
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
                a.Property(e => e.EndDate).HasColumnType("date").HasColumnName("EndDate")
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            });
            
            builder.Ignore(e => e.StatementId);
            builder.HasOne<AdjustmentAggregate>().WithMany(u => u.Statements).HasForeignKey(u => u.LoanId);
            builder.HasOne<Loan>().WithMany().HasForeignKey(u => u.LoanId);
        }
    }
}
