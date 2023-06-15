using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class AdjustmentCodeEntityConfiguration : BaseConfiguration<AdjustmentCode>
    {
        public override void Configure(EntityTypeBuilder<AdjustmentCode> builder)
        {
            base.Configure(builder);
            builder.ToTable("StatementAdjustmentCodes", "loan");
            builder.HasKey(e => e.AdjCodeID).HasName("PK__Statemen__93408B11C2C48912");

            builder.Property(e => e.AdjCodeID).HasColumnType("int").HasColumnName("AdjCodeID").ValueGeneratedNever();
            builder.Property(e => e.AdjustmentType).HasColumnType("varchar").HasColumnName("LongStr");
            
           // builder.HasOne(u => u.Loan).WithOne().HasForeignKey<Loan>(u => u.LoanId);
            

        }
    }
}
