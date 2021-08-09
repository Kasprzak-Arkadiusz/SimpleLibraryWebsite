using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.ModelConfiguration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.BookId);
            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(60);
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(120);
            builder.Property(b => b.Genre)
                .IsRequired();
            builder.Property(b => b.DateOfAdding)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("Date of adding");
            builder.Property(b => b.IsBorrowed)
                .IsRequired()
                .HasColumnType("bit")
                .HasColumnName("Is borrowed?");
        }
    }
}
