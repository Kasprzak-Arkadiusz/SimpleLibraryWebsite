using System;
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
        public DateTime LentFrom { get; set; }
        [Display(Name = "Lent to")]
        public DateTime LentTo { get; set; }
    }
}
