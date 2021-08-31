using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.ModelConfiguration
{
    public class ReaderConfiguration : IEntityTypeConfiguration<Reader>
    {
        public void Configure(EntityTypeBuilder<Reader> builder)
        {
            builder.HasKey(r => r.ReaderId);
            builder.Property(r => r.FirstName)
                .IsRequired()
                .HasMaxLength(60);
            builder.Property(r => r.LastName)
                .IsRequired()
                .HasMaxLength(60);
        }
    }
}
