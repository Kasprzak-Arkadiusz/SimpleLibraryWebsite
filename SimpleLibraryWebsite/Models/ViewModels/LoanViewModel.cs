using X.PagedList;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class LoanViewModel
    {
        public IPagedList<Loan> PaginatedList { get; init; }
        public string ReaderFirstName { get; init; }
        public string ReaderLastName { get; init; }
        public string BookTitle { get; init; }
    }
}
