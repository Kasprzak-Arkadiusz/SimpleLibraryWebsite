using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }
        [Display(Name = "Book title")]
        public string Title { get; set; }
        [Display(Name = "Book author")]
        public string Author { get; set; }
        public Genres Genre { get; set; }

        public string ReaderId { get; set; }
        public Reader Reader { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Request(string readerId, string title, string author, Genres genre)
        {
            ReaderId = readerId;
            Title = title;
            Author = author;
            Genre = genre;
        }

        public Request() { }

        public bool AnyFieldIsNullOrEmpty()
        {
            return Author is null || Title is null;
        }
    }
}
