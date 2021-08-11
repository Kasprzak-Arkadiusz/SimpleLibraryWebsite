using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Reader
    {
        [Key] 
        public string ReaderId { get; set; }
        [Display(Name = "Reader name")] 
        /*[StringLength(60, MinimumLength = 1)]
        [Required]*/
        public string FirstName { get; set; }
        [Display(Name = "Reader surname")]
        /*[StringLength(60, MinimumLength = 1)]
        [Required]*/
        public string LastName { get; set; }
        [Display(Name = "Number of loans")]
        public int NumberOfLoans { get; set; } = 0;
        [Display(Name = "Number of requests")]
        public int NumberOfRequests { get; set; } = 0;

        public User User { get; set; }
        public string Id { get; set; }
        public ICollection<Loan> Loans { get; set; }
        public ICollection<Request> Requests { get; set; }

        public Reader(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public Reader() { }
    }
}