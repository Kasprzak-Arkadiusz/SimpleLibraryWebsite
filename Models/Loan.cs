using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Loan
    {
        public const int DaysOfLoan = 21;

        [Key]
        public int LoanId { get; set; }
        [Display(Name = "Lent from")]
        [DataType(DataType.Date)]
        public DateTime LentFrom { get; set; }
        [Display(Name = "Lent to")]
        [DataType(DataType.Date)]
        public DateTime LentTo { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
        public string ReaderId { get; set; }
        public Reader Reader { get; set; }

        public Loan(int bookId, string readerId, DateTime lentFrom)
        {
            BookId = bookId;
            ReaderId = readerId;
            LentFrom = lentFrom;
            TimeSpan time = new TimeSpan(DaysOfLoan, 0, 0, 0);
            LentTo = lentFrom.Add(time);
        }

        public Loan(int bookId, string readerId)
        {
            BookId = bookId;
            ReaderId = readerId;
            LentFrom = DateTime.Today;
            TimeSpan time = new TimeSpan(DaysOfLoan, 0, 0, 0);
            LentTo = DateTime.Today.Add(time);
        }

        public Loan() { }
    }
}
