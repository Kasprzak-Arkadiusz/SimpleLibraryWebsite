using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;

namespace SimpleLibraryWebsite.Models
{
    public class Reader
    {
        [Key]
        public int ReaderId { get; set; }
        [Display(Name = "Reader name")]
        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Reader surname")]
        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Surname { get; set; }

        public Reader(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        public Reader() { }

        public async Task<ICollection<Loan>> GetLoans(ApplicationDbContext context)
        {
            return await context.Loans.Where(l => l.ReaderId == ReaderId).ToListAsync();
        }

        public async Task<ICollection<Request>> GetRequests(ApplicationDbContext context)
        {
            return await context.Requests.Where(r => r.ReaderId == ReaderId).ToListAsync();
        }
    }
}
