using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SimpleLibraryWebsite.Models
{
    [NotMapped]
    public class User : IdentityUser
    {
        [Display(Name = "Reader name")]
        public string FirstName { get; set; }
        [Display(Name = "Reader surname")]
        public string LastName { get; set; }

        public Reader Reader { get; set; }

        public User(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public User() { }
    }
}
