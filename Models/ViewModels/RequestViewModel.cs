using System.Collections.Generic;
using X.PagedList;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class RequestViewModel
    {
        public IPagedList<Request> PaginatedList { get; set; }
        public List<Request> Requests { get; set; }
        public string Author { get; set; }
        public string BookTitle { get; set; }
    }
}
