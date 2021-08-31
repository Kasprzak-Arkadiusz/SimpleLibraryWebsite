using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models.ViewModels
{
    public class BookBorrowViewModel
    {
        public Book BorrowedBook { get; set; }
        [Display(Name = "Return date")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }

        public BookBorrowViewModel(Book borrowedBook)
        {
            BorrowedBook = borrowedBook;
            ReturnDate = DateTime.Today.Add(new TimeSpan(14, 0, 0, 0));
        }
    }
}
