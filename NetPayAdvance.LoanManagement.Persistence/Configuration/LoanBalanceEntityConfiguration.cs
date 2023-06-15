using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration;

public class LoanBalanceEntityConfiguration : BaseConfiguration<LoanBalance>
{
    public override void Configure(EntityTypeBuilder<LoanBalance> builder)
    {
        base.Configure(builder);
        builder.ToTable("vLoanBalance", "loan");
        builder.HasKey(e => e.LoanId);
        builder.Property(e => e.LoanId).HasColumnName("LoanID");

        builder.OwnsOne(x => x.Amount, a =>
        {
            a.Property(e => e.Principal).HasColumnType("decimal").HasColumnName("Principal");
            a.Property(e => e.Interest).HasColumnType("decimal").HasColumnName("Interest");
            a.Property(e => e.CabFees).HasColumnType("decimal").HasColumnName("CabFee");
            a.Property(e => e.Nsf).HasColumnType("decimal").HasColumnName("NSF");
            a.Property(e => e.LateFees).HasColumnType("decimal").HasColumnName("Fees");
        });
        builder.HasOne<Loan>().WithOne(u => u.Balance).HasForeignKey<LoanBalance>(u => u.LoanId);
    }
}