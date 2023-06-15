using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class LoanEntityConfiguration : BaseConfiguration<Loan>
    {
        public override void Configure(EntityTypeBuilder<Loan> builder)
        {       
            var converter = new ValueConverter<LoanType, byte>(v => (byte)v, v => (LoanType)v);

            base.Configure(builder);
            builder.ToTable("Loans", "loan");
            builder.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD43778E2007B");

            builder.Property(e => e.LoanId).HasColumnName("LoanID").ValueGeneratedOnAdd();
            builder.Property(e => e.PowerID).HasColumnName("PowerID");
            builder.Property(e => e.TransID).HasColumnName("TransID");
            builder.Property(e => e.CreatedOn).HasColumnType("datetime").HasColumnName("CreatedOn");
            builder.Property(e => e.StartedOn).HasColumnType("datetime").HasColumnName("StartedOn");
            builder.Property(e => e.ClosedOn).HasColumnType("datetime").HasColumnName("CompletedOn");
            builder.Property(e => e.AccountingClosedOn).HasColumnType("datetime").HasColumnName("AccountingClosedOn");
            builder.Property(e => e.IsCancelled).HasColumnType("bit").HasColumnName("IsCancelled");
            builder.Property(e => e.IsRescinded).HasColumnType("bit").HasColumnName("IsRescended");

            builder.OwnsOne(e => e.Config, e =>
            {
                e.Property(e => e.Location).HasColumnName("Location");
                e.Property(e => e.CreditLimit).HasColumnType("decimal").HasColumnName("Amount");
                e.Property(e => e.Apr).HasColumnType("decimal").HasColumnName("APR");
                e.Property(e => e.CabApr).HasColumnType("decimal").HasColumnName("CABAPR");
                e.Property(e => e.LoanType).HasColumnName("TypeID").HasConversion(converter);
            });
            builder.Ignore(e => e.Status);
            builder.Ignore(e => e.LoanInfo);
            
            //builder.HasOne(u => u.Balance).WithOne().HasForeignKey<Loan>(u => u.LoanID);
        }
    }
}
