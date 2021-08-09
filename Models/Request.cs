using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }
        public int ReaderId { get; set; }
        [Display(Name = "Book title")]
        [StringLength(120, MinimumLength = 1)]
        [Required]
        public string Title { get; set; }
        [Display(Name = "Book author")]
        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Author { get; set; }
        [Required]
        public Genres Genre { get; set; }
        [Display(Name = "Number of upvotes")]
        [Range(0, int.MaxValue)]
        public int NumberOfUpvotes { get; set; }

        public User Reader { get; set; }

        public Request(int readerId, string title, string author, Genres genre)
        {
            ReaderId = readerId;
            Title = title;
            Author = author;
            Genre = genre;
            NumberOfUpvotes = 0;
        }
        public Request() { }

        public void FillMissingProperties(User reader)
        {
            Reader = reader;
            NumberOfUpvotes = 0;
        }
    }
}
