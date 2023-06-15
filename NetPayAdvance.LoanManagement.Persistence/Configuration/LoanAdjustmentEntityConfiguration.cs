using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class LoanAdjustmentEntityConfiguration : BaseConfiguration<LoanAdjustment>
    {
        public override void Configure(EntityTypeBuilder<LoanAdjustment> builder)
        {
            base.Configure(builder);
            builder.ToTable("LoanAdjustments", "loan");
            builder.HasKey(e => e.AdjustmentID).HasName("LoanAdjustments_pk");

            builder.Property(e => e.AdjustmentID).HasColumnName("AdjustmentID").ValueGeneratedOnAdd();
            builder.Property(e => e.LoanID).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.Property(e => e.PaymentID).HasColumnType("int").HasColumnName("PaymentID").ValueGeneratedNever();
            builder.Property(e => e.Teller).HasColumnType("char").HasColumnName("Teller").HasMaxLength(3).IsUnicode(false);
            
            builder.OwnsOne(x => x.Adjustment, a =>
            {
                a.Property(e => e.AdjustmentCodeID).HasColumnType("int").HasColumnName("AdjCodeID");
                a.OwnsOne(x => x.Amount, ca =>
                {
                    ca.Property(e => e.Principal).HasColumnType("decimal").HasColumnName("Principal");
                    ca.Property(e => e.Interest).HasColumnType("decimal").HasColumnName("Interest");
                    ca.Property(e => e.CabFees).HasColumnType("decimal").HasColumnName("CabFees");
                    ca.Property(e => e.Nsf).HasColumnType("decimal").HasColumnName("NSF");
                    ca.Property(e => e.LateFees).HasColumnType("decimal").HasColumnName("LateFees");
                });
               
                a.Property(e => e.CreatedOn).HasColumnType("datetime").HasColumnName("CreatedOn");
                
            });

            builder.HasMany(e => e.StatementAdjustments).WithOne().HasForeignKey(e => e.LoanAdjustmentID);
        }
    }
}
