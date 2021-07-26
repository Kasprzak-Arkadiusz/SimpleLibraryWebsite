using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public Book Book { get; set; }
        public Reader Reader { get; set; }

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
