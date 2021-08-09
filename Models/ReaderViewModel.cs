using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models
{
    public class ReaderViewModel
    {
        public PaginatedList<User> PaginatedList { get; set; }
        public List<User> Readers { get; set; }
        public string ReaderName { get; set; }
        public string ReaderSurname { get; set; }
    }
}
