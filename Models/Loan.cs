using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLibraryWebsite.Models
{
    public class Loan
    {
        [Key]
        public int LoanID { get; set; }
        public int BookID { get; set; }
        public int ReaderID { get; set; }
        [Display(Name = "Lent from")]
        [DataType(DataType.Date)]
        public DateTime LentFrom { get; set; }
        [Display(Name = "Lent to")]
        [DataType(DataType.Date)]
        public DateTime LentTo { get; set; }

        public Loan(int bookId, int readerId, DateTime lentFrom)
        {
            BookID = bookId;
            ReaderID = readerId;
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
