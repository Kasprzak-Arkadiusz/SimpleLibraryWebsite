using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models
{
    public class RequestViewModel
    {
        public List<Request> Requests { get; set; }
        public string ReaderName { get; set; }
        public string ReaderSurname { get; set; }
        public string BookTitle { get; set; }
    }
}
