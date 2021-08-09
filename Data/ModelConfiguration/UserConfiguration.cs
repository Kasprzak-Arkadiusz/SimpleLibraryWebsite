using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.ModelConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(r => r.UserId);
            builder.Property(r => r.FirstName)
                .IsRequired()
                .HasMaxLength(60);
            builder.Property(r => r.LastName)
                .IsRequired()
                .HasMaxLength(60);
        }
    }
}
