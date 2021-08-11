using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class RequestViewModel
    {
        public PaginatedList<Request> PaginatedList { get; set; }
        public List<Request> Requests { get; set; }
        public string Author { get; set; }
        public string BookTitle { get; set; }
    }
}
