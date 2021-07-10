using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public enum Genres { Adventure, Novel,  SciFi, Fantasy, Romance, Thriller, Horror, Biography}

public class Book
    {
        [Key]
        public int BookID { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public Genres? Genre { get; set; }
        [Display(Name  = "Date of adding")]
        public DateTime AddingDate { get; set; }
        public bool IsBorrowed { get; set; }
    }
}
