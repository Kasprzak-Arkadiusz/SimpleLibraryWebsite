using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models
{
    public class ReaderViewModel
    {
        public List<Reader> Readers { get; set; }

        public string ReaderName { get; set; }

        public string ReaderSurname { get; set; }
    }
}
