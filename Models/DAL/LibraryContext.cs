using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SimpleLibraryWebsite.Models.DAL
{
    public class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}
