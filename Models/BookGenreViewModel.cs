using Microsoft.AspNetCore.Mvc.Rendering;

namespace SimpleLibraryWebsite.Models
{
    public class BookGenreViewModel
    {
        public SelectList Genres { get; set; }
        public PaginatedList<Book> PaginatedList { get; set; }  
        public string BookGenre { get; set; }
        public string BookTitle { get; set; }
    }
}
