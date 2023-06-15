using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Converters;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class BillingStatementEntityConfiguration : BaseConfiguration<BillingStatement>
    {
        public override void Configure(EntityTypeBuilder<BillingStatement> builder)
        {
            builder.ToTable("BillingStatements", "loan");
            builder.HasKey(e => new { e.LoanID, e.OrigDueDate });

            builder.Property(e => e.LoanID).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.Property(e => e.OrigDueDate).HasColumnType("datetime").HasColumnName("OrigDueDate")
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            builder.Property(e => e.StatementHTML).HasColumnName("StatementHTML");
        }
    }
}
