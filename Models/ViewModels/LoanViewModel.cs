using System.Collections.Generic;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class LoanViewModel
    {
        public PaginatedList<Loan> PaginatedList { get; set; }
        public List<Loan> Loans { get; set; }
        public string ReaderName { get; set; }
        public string ReaderLastName { get; set; }
        public string BookTitle { get; set; }
    }
}
