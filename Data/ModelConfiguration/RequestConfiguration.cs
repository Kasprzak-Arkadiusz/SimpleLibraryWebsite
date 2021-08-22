using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.ModelConfiguration
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.RequestId);
            builder.Property(r => r.Author)
                .IsRequired()
                .HasMaxLength(60);
            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(120);
            builder.Property(r => r.Genre)
                .IsRequired();
            builder.Property(r => r.RowVersion)
                .IsRowVersion();
        }
    }
}
