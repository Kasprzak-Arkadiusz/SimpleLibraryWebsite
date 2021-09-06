using X.PagedList;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class RequestViewModel
    {
        public IPagedList<Request> PaginatedList { get; init; }
        public string Author { get; init; }
        public string BookTitle { get; init; }
    }
}
