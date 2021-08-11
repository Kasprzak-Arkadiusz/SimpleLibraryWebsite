using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public class Reader
    {
        public const int BookLoansLimit = 5;
        public const int BookRequestLimit = 5;

        [Key] 
        public string ReaderId { get; set; }
        [Display(Name = "Reader name")]
        public string FirstName { get; set; }
        [Display(Name = "Reader surname")]
        public string LastName { get; set; }
        [Display(Name = "Number of loans")]
        public int NumberOfLoans { get; set; }
        [Display(Name = "Number of requests")]
        public int NumberOfRequests { get; set; }

        public User User { get; set; }
        public string Id { get; set; }
        public ICollection<Loan> Loans { get; set; }
        public ICollection<Request> Requests { get; set; }

        public Reader() { }
        public Reader(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public Reader(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            ReaderId = user.Id;
        }
        
    }
}