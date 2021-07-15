using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }
        public int ReaderID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genres Genre { get; set; }
        [Display(Name = "Number of upvotes")]
        public int NumberOfUpvotes { get; set; }
        public virtual Reader Reader { get; set; }

        public Request(int readerId, string title, string author, Genres genre)
        {
            ReaderID = readerId;
            Title = title;
            Author = author;
            Genre = genre;
        }
        public Request() { }
    }
}
