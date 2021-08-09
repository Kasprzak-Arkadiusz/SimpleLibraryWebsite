using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Librarian.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Reader.ToString()));
        }

        public static async Task SeedAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new User()
            {
                UserName = "superadmin",
                Email = "superadmin@gmail.com",
                Name = "James",
                Surname = "Smith",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word.");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
        }
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Books.Any())
            {
                return;
            }

            var books = new Book[]
            {
                new Book("Adam Mickiewicz", "Master Thaddeus",Genres.Poetry),
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

            var readers = new User[]
            {
                new User("John", "Smith"),
                new User("Jimmy", "Johnson"),
                new User("Emily", "Richardson"),
                new User("Elisabeth", "Lee")
            };

            foreach (User r in readers)
            {
                context.Readers.Add(r);
            }

            context.SaveChanges();

            var loans = new Loan[]
            {
                new Loan(1, 1,  DateTime.Now),
                new Loan(4, 1, DateTime.Parse("01/02/2021")),
                new Loan(6, 3, DateTime.Parse("15/04/2021")),
                new Loan(7, 3, DateTime.Parse("16/05/2021")),
            };

            foreach (Loan l in loans)
            {
                context.Loans.Add(l);
            }

            var requests = new Request[]
            {
                new Request(1, "For Whom the Bell Tolls", "Ernest Hemingway", Genres.Novel),
                new Request(2, "The Hobbit, or There and Back Again", "J.R.R. Tolkien", Genres.Fantasy)
            };

            foreach (Request r in requests)
            {
                context.Requests.Add(r);
            }

            context.SaveChanges();
        }
    }
}
