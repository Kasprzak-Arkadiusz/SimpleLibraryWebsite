using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Librarian.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Reader.ToString()));
        }

        public static async Task SeedAdminAsync(UserManager<User> userManager)
        {
            var defaultUser = new User()
            {
                UserName = "superadmin",
                Email = "superadmin@gmail.com",
                FirstName = "James",
                LastName = "Smith",
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
        public static void Initialize(UserManager<User> userManager, ApplicationDbContext context)
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

            var readers = new Reader[]
            {
                new Reader("John", "Smith"),
                new Reader("Jimmy", "Johnson"),
                new Reader("Emily", "Richardson"),
                new Reader("Elisabeth", "Lee")
            };

            User[] users = new User[readers.Length];

            foreach (Reader r in readers)
            {
                context.Readers.Add(r);
            }

            context.SaveChanges();

            for (int i = 0; i < users.Length; i++)
            {
                users[i] = new User
                {
                    UserName = "user" + i,
                    Email = "me" + i + "@gmail.com",
                    FirstName = "name" + i,
                    LastName = "surname" + i,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Id = readers[i].ReaderId.ToString()
                };
                userManager.CreateAsync(users[i], "123Pa$$word.").Wait();
                userManager.AddToRoleAsync(users[i], Roles.Reader.ToString()).Wait();
            }


            var loans = new Loan[]
            {
                new Loan(1, readers[0].ReaderId,  DateTime.Now),
                new Loan(4, readers[1].ReaderId, DateTime.Parse("01/02/2021")),
                new Loan(6, readers[3].ReaderId, DateTime.Parse("15/04/2021")),
                new Loan(7, readers[3].ReaderId, DateTime.Parse("16/05/2021")),
            };

            foreach (Loan l in loans)
            {
                context.Loans.Add(l);
            }

            var requests = new Request[]
            {
                new Request(readers[1].ReaderId, "For Whom the Bell Tolls", "Ernest Hemingway", Genres.Novel),
                new Request(readers[2].ReaderId, "The Hobbit, or There and Back Again", "J.R.R. Tolkien", Genres.Fantasy)
            };

            foreach (Request r in requests)
            {
                context.Requests.Add(r);
            }

            context.SaveChanges();
        }

        public static async Task CreateUsers(UserManager<User> userManager, ApplicationDbContext context)
        {
            var readers = await context.Readers.ToArrayAsync();

            User[] users = new User[readers.Length];

            for (int i = 0; i < users.Length; i++)
            {
                users[i] = new User
                {
                    UserName = "user" + i,
                    Email = "me" + i + "@gmail.com",
                    FirstName = "name" + i,
                    LastName = "surname" + i,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Id = readers[i].id
                };
                await userManager.CreateAsync(users[i], "123Pa$$word.");
                userManager.AddToRoleAsync(users[i], Roles.Reader.ToString()).Wait();
            }
        }
    }
}
