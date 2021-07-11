using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Book { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}
