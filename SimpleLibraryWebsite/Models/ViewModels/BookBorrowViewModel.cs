using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class BookBorrowViewModel
    {
        public Book BorrowedBook { get; init; }
        [Display(Name = "Return date")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; init; }

        public BookBorrowViewModel(Book borrowedBook)
        {
            BorrowedBook = borrowedBook;
            ReturnDate = DateTime.Today.Add(new TimeSpan(14, 0, 0, 0));
        }
    }
}
