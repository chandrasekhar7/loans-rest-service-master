using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Persistence.Converters;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class StatementAdjustmentEntityConfiguration : BaseConfiguration<StatementAdjustment>
    {
        public override void Configure(EntityTypeBuilder<StatementAdjustment> builder)
        {
            base.Configure(builder);
            builder.ToTable("StatementAdjustmentHistory", "loan");
            builder.HasKey(e => e.AdjustmentID).HasName("PK__Statemen__E60DB8B38DC93433");

            builder.Property(e => e.Teller).HasColumnName("Teller").HasColumnType("char").HasMaxLength(3);
            builder.Property(e => e.AdjustmentID).HasColumnName("AdjustmentID").ValueGeneratedOnAdd();
            builder.Property(e => e.LoanID).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.OwnsOne(x => x.Adjustment, a =>
            {
                a.Property(e => e.AdjustmentCodeID).HasColumnType("int").HasColumnName("AdjCodeID");
              //  a.Ignore(e => e.AdjustmentCode);
                a.Property(e => e.CreatedOn).HasColumnType("datetime").HasColumnName("CreatedOn");

                a.OwnsOne(x => x.Amount, ca =>
                {
                    ca.Property(e => e.Principal).HasColumnType("decimal").HasColumnName("Principal");
                    ca.Property(e => e.Interest).HasColumnType("decimal").HasColumnName("Interest");
                    ca.Property(e => e.CabFees).HasColumnType("decimal").HasColumnName("CabFee");
                    ca.Property(e => e.Nsf).HasColumnType("decimal").HasColumnName("NSF");
                    ca.Property(e => e.LateFees).HasColumnType("decimal").HasColumnName("Fees");
                });
            });
         
            builder.Property(e => e.OrigDueDate).HasColumnType("date").HasColumnName("PaymentDueDate")
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            builder.Property(e => e.LoanAdjustmentID).HasColumnType("int").HasColumnName("LoanAdjustmentID").ValueGeneratedNever();
        }
    }
}
