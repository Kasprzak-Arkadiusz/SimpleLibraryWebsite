using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models
{
    public class ReaderViewModel
    {
        public PaginatedList<Reader> PaginatedList { get; set; }
        public List<Reader> Readers { get; set; }
        public string ReaderName { get; set; }
        public string ReaderLastName { get; set; }
    }
}
