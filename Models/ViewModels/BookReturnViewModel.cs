namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class BookReturnViewModel
    {
        public Loan Loan { get; set; }
        public Book ReturnedBook { get; set; }

        public Reader Reader { get; set; }

        public BookReturnViewModel(Loan loan, Book returnedBook, Reader reader)
        {
            Loan = loan;
            ReturnedBook = returnedBook;
            Reader = reader;
        }
    }
}