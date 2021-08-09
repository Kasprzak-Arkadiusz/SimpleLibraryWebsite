using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;

namespace SimpleLibraryWebsite.Models
{
    public class Reader
    {
        [Key] 
        public Guid ReaderId { get; set; }
        [Display(Name = "Reader name")] 
        /*[StringLength(60, MinimumLength = 1)]
        [Required]*/
        public string FirstName { get; set; }
        [Display(Name = "Reader surname")]
        /*[StringLength(60, MinimumLength = 1)]
        [Required]*/
        public string LastName { get; set; }

        public User User { get; set; }
        public string id { get; set; }
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