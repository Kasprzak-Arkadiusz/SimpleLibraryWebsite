using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.ModelConfiguration
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.HasKey(l => l.LoanId);
            builder.Property(l => l.LentFrom)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("Lent from");
            builder.Property(l => l.LentTo)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("Lent to");
        }
    }
}
