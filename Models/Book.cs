using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLibraryWebsite.Models
{
    public enum Genres { Adventure, Novel, SciFi, Fantasy, Romance, Thriller, Horror, Biography, Poetry, Scientific }
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        [Display(Name = "Book author")]
        public string Author { get; set; }
        [Display(Name = "Book title")]
        public string Title { get; set; }
        public Genres? Genre { get; set; }
        [Display(Name = "Date of adding")]
        [DataType(DataType.Date)]
        public DateTime AddingDate { get; set; }
        public bool IsBorrowed { get; set; }

        public Book(string author, string title, Genres genre)
        {
            Author = author;
            Title = title;
            Genre = genre;
            AddingDate = DateTime.Now;
            IsBorrowed = false;
        }

        public Book()
        {
        }

        public void FillMissingProperties()
        {
            AddingDate = DateTime.Now;
            IsBorrowed = false;
        }
    }
}
