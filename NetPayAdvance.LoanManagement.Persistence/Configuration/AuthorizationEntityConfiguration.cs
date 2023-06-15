using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;

namespace NetPayAdvance.LoanManagement.Persistence.Configuration
{
    public class AuthorizationEntityConfiguration : BaseConfiguration<Authorization>
    {
        public override void Configure(EntityTypeBuilder<Authorization> builder)
        {
            var converter = new ValueConverter<PendingChangeType, int>(v => (int)v, v => (PendingChangeType)v);

            base.Configure(builder);
            builder.ToTable("AuthorizationChanges", "loan");
            builder.HasKey(e => e.LoanChangeId).HasName("PK__Authoriz__9BABD701D70B0C35");

            builder.Property(e => e.LoanChangeId).HasColumnName("LoanChangeID").ValueGeneratedOnAdd();
            builder.Property(e => e.CustomerChangeId).HasColumnType("int").HasColumnName("CustomerChangeID").ValueGeneratedNever();
            builder.Property(e => e.LoanId).HasColumnType("int").HasColumnName("LoanID").ValueGeneratedNever();
            builder.Property(e => e.AddendumId).HasColumnType("int").HasColumnName("AddendumID").ValueGeneratedNever();
            builder.Property(e => e.CreatedOn).HasColumnType("datetime").HasColumnName("CreatedOn");
            builder.Property(e => e.CreatedBy).HasColumnType("char").HasMaxLength(3).HasColumnName("CreatedBy").IsUnicode(false);
            builder.Property(e => e.ChangeType).HasColumnType("int").HasColumnName("ChangeType").HasConversion(converter);
            builder.Property(e => e.NewValue).HasColumnType("varchar").HasColumnName("NewValue");
            builder.Property(e => e.CancelledOn).HasColumnType("datetime").HasColumnName("CancelledOn");
            builder.Property(e => e.CancelledBy).HasColumnName("CancelledBy");
            builder.Property(e => e.CompletedOn).HasColumnType("datetime").HasColumnName("CompletedOn");
            builder.Property(e => e.CompletedBy).HasColumnName("CompletedBy");
        }
    }
}
