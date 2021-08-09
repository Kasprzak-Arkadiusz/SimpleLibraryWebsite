using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }
        [Display(Name = "Lent from")]
        //[DataType(DataType.Date)]
        public DateTime LentFrom { get; set; }
        [Display(Name = "Lent to")]
        //[DataType(DataType.Date)]
        public DateTime LentTo { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
        public Guid ReaderId { get; set; }
        public Reader Reader { get; set; }

        public Loan(int bookId, Guid readerId, DateTime lentFrom)
        {
            BookId = bookId;
            ReaderId = readerId;
            LentFrom = lentFrom;
            TimeSpan time = new TimeSpan(14, 0, 0, 0);
            LentTo = lentFrom.Add(time);
        }

        public Loan() { }

        public void FillMissingFields()
        {
            LentFrom = DateTime.Now;
            TimeSpan time = new TimeSpan(14, 0, 0, 0);
            LentTo = LentFrom.Add(time);
        }
    }
}
