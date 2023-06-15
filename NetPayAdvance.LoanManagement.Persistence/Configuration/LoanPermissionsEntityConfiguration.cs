using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class LoanPermissionsEntityConfiguration : BaseConfiguration<LoanPermissions>
    {
        public override void Configure(EntityTypeBuilder<LoanPermissions> builder)
        {
            base.Configure(builder);
            builder.ToTable("LoanPermissions", "loan");
            builder.HasKey(e => e.LoanId).HasName("PK__LoanPerm__4F5AD457479E35BC");
            
            builder.Property(e => e.LoanId).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.Property(e => e.AutoDebit).HasColumnType("bit").HasColumnName("AutoDebit").ValueGeneratedNever();
            builder.Property(e => e.AutoACH).HasColumnType("bit").HasColumnName("AutoACH");
            
            builder.HasOne<Loan>().WithOne(u => u.Permissions).HasForeignKey<LoanPermissions>(u => u.LoanId);
        }
    }
}
