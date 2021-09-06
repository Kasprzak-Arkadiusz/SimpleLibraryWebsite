using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class BookGenreViewModel
    {
        public SelectList Genres { get; init; }
        public IPagedList<Book> PaginatedList { get; init; }  
        public string BookGenre { get; init; }
        public string BookTitle { get; init; }
    }
}
