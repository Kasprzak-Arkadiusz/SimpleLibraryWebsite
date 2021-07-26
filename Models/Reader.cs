using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Reader
    {
        [Key]
        public int ReaderID { get; set; }
        [Display(Name = "Reader name")]
        public string Name { get; set; }
        [Display(Name = "Reader surname")]
        public string Surname { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<Request> Requests { get; set; }

        public Reader(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        public Reader() { }
    }
}
