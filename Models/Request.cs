using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }
        public int ReaderID { get; set; }
        public int BookID { get; set; }
        [Display(Name = "Number of upvotes")]
        public int NumberOfUpvotes { get; set; }
        public virtual Reader Reader { get; set; }
        public virtual Book Book { get; set; }
    }
}
