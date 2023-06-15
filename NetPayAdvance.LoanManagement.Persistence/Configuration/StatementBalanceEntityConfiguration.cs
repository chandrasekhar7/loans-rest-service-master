using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Converters;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration;

public class StatementBalanceEntityConfiguration : BaseConfiguration<StatementBalance>
{
    public override void Configure(EntityTypeBuilder<StatementBalance> builder)
    {
        base.Configure(builder);
        builder.ToView("vStatementBalance", "loan");
        builder.HasKey(e => new {e.LoanId, e.OrigDueDate});
        builder.Property(e => e.LoanId).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
        builder.Property(e => e.OrigDueDate).HasColumnType("date").HasColumnName("OrigDueDate")
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();

        builder.OwnsOne(x => x.Amount, a =>
        {
            a.Property(e => e.Principal).HasColumnType("decimal").HasColumnName("Principal");
            a.Property(e => e.Interest).HasColumnType("decimal").HasColumnName("Interest");
            a.Property(e => e.CabFees).HasColumnType("decimal").HasColumnName("CabFee");
            a.Property(e => e.Nsf).HasColumnType("decimal").HasColumnName("NSF");
            a.Property(e => e.LateFees).HasColumnType("decimal").HasColumnName("Fees");
        });
        builder.HasOne<Statement>().WithOne(u => u.Balance).HasForeignKey<StatementBalance>(u => new {u.LoanId, u.OrigDueDate});
    }
}