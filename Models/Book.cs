using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public enum Genres { Adventure, Novel, SciFi, Fantasy, Romance, Thriller, Horror, Biography, Poetry, Scientific, Essay}
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Display(Name = "Book author")]
        public string Author { get; set; }
        [Display(Name = "Book title")]
        public string Title { get; set; }
        [Required]
        public Genres Genre { get; set; }
        [Display(Name = "Date of adding")]
        [DataType(DataType.Date)]
        public DateTime DateOfAdding { get; set; }
        [Display(Name = "Is book borrowed?")]
        public bool IsBorrowed { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Book(string author, string title, Genres genre)
        {
            Author = author.Trim();
            Title = title.Trim();
            Genre = genre;
            DateOfAdding = DateTime.Now;
            IsBorrowed = false;
        }

        public Book()
        {
            DateOfAdding = DateTime.Now;
            IsBorrowed = false;
        }

        public bool AnyFieldIsNullOrEmpty()
        {
            return Author is null || Title is null;
        }
    }
}
