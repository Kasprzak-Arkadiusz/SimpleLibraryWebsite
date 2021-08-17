using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class BookGenreViewModel
    {
        public SelectList Genres { get; set; }
        public IPagedList<Book> PaginatedList { get; set; }  
        public string BookGenre { get; set; }
        public string BookTitle { get; set; }
    }
}
