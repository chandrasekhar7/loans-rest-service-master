using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class LoanContractsEntityConfiguration : BaseConfiguration<LoanContracts>
    {
        public override void Configure(EntityTypeBuilder<LoanContracts> builder)
        {
            base.Configure(builder);
            builder.ToTable("Contracts", "loan");
            builder.HasKey(e => e.LoanId).HasName("PK_LoanID_DateSigned");
            
            builder.Property(e => e.LoanId).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.Property(e => e.DateSigned).HasColumnType("datetime").HasColumnName("DateSigned").ValueGeneratedNever();
            
            builder.HasOne<Loan>().WithOne(u => u.Contracts).HasForeignKey<LoanContracts>(u => u.LoanId);

        }
    }
}
