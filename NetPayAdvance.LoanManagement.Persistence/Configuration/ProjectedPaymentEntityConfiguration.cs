using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Converters;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class ProjectedPaymentsEntityConfiguration : BaseConfiguration<ProjectedPayment>
    {
        public override void Configure(EntityTypeBuilder<ProjectedPayment> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProjectedPayments", "loan").HasKey(e => new { e.LoanID, e.OrigDueDate });

            builder.Property(e => e.LoanID).HasColumnType("int").HasColumnName("LoanID");
            builder.Property(e => e.OrigDueDate).HasColumnType("date").HasColumnName("OrigDueDate")
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            builder.Property(e => e.RemainingPrincipal).HasColumnType("decimal").HasColumnName("RemainingPrincipal");
            builder.Property(e => e.Payment).HasColumnType("decimal").HasColumnName("Payment");
            builder.Property(e => e.Skipped).HasColumnType("bit").HasColumnName("Skipped");

            builder.OwnsOne(x => x.Period, a =>
            {
                a.Property(e => e.StartDate).HasColumnType("date").HasColumnName("StartDate")
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
                a.Property(e => e.EndDate).HasColumnType("date").HasColumnName("EndDate")
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            });
            
            builder.OwnsOne(x => x.Amount, a =>
            {
                a.Property(e => e.Principal).HasColumnType("decimal").HasColumnName("Principal");
                a.Property(e => e.Interest).HasColumnType("decimal").HasColumnName("Interest");
                a.Property(e => e.CabFees).HasColumnType("decimal").HasColumnName("CabFee");
            });
            builder.HasOne<Loan>()
                .WithMany(u => u.ProjectedPayments)      
                .HasForeignKey(c => c.LoanID);
            //builder.HasOne(u => u).WithOne().HasForeignKey<Loan>(u => u.LoanID);
        }
    }
}
