using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Books.Any())
            {
                return;
            }

            var books = new Book[]
            {
                new Book{Author = "Adam Mickiewicz", Title = "Master Thaddeus",Genre = Genres.Poetry, AddingDate= DateTime.Now, IsBorrowed = true},
                new Book("Adam Mickiewicz","Ode to Youth", Genres.Poetry),
                new Book("Stephen King",  "It",  Genres.Horror),
                new Book("Jules Verne", "Twenty Thousand Leagues Under the Seas", Genres.Adventure),
                new Book("Herman Melville","Moby Dick", Genres.Novel),
                new Book("George Orwell", "Nineteen Eighty-Four", Genres.SciFi),
                new Book("J.R.R. Tolkien", "Lord of the rings", Genres.Fantasy),
                new Book("Jane Austen", "Pride and Prejudice", Genres.Romance),
                new Book("Walter Isaacson", "Steve Jobs", Genres.Biography)
            };

            foreach (Book b in books)
            {
                context.Books.Add(b);
            }

            var readers = new Reader[]
            {
                new Reader{Name = "John", Surname = "Smith"},
                new Reader("Jimmy", "Johnson"),
                new Reader("Emily", "Richardson"),
                new Reader("Elisabeth", "Lee")
            };

            foreach (Reader r in readers)
            {
                context.Readers.Add(r);
            }

            context.SaveChanges();

            var loans = new Loan[]
            {
                new Loan{BookID = 1,ReaderID = 1, LentFrom = DateTime.Now},
                new Loan(4, 1, DateTime.Parse("01/02/2021")),
                new Loan(6, 3, DateTime.Parse("15/04/2021")),
                new Loan(7, 3, DateTime.Parse("16/05/2021")),
            };

            foreach (Loan l in loans)
            {
                context.Loans.Add(l);
            }

            /*var Requests = new Request[]
            {

            };*/

            context.SaveChanges();
        }
    }
}
