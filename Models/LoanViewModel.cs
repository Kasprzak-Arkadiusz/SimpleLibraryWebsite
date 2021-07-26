using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models
{
    public class LoanViewModel
    {
        public List<Loan> Loans { get; set; }
        public string ReaderName { get; set; }
        public string ReaderSurname { get; set; }
        public string BookTitle { get; set; }
    }
}
