﻿using System;
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
        public DateTime LentFrom { get; set; }
        [Display(Name = "Lent to")]
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
    }
}
