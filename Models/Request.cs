using System.ComponentModel.DataAnnotations;
using SimpleLibraryWebsite.Models.Authorization;

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
        [Display(Name = "Number of upvotes")]
        [Range(0, int.MaxValue)]
        public uint NumberOfUpvotes { get; set; }

        public string ReaderId { get; set; }
        public Reader Reader { get; set; }

        public Request(string readerId, string title, string author, Genres genre)
        {
            ReaderId = readerId;
            Title = title;
            Author = author;
            Genre = genre;
            NumberOfUpvotes = 0;
        }

        public Request()
        {
            
        }

        public bool AnyFieldIsNullOrEmpty()
        {
            return Author is null || Title is null;
        }
    }
}
