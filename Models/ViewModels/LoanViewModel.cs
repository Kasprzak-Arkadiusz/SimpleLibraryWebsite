using X.PagedList;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class LoanViewModel
    {
        public IPagedList<Loan> PaginatedList { get; set; }
        public string ReaderFirstName { get; set; }
        public string ReaderLastName { get; set; }
        public string BookTitle { get; set; }
    }
}
