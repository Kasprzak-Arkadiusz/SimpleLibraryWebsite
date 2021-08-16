using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.Authorization;

namespace SimpleLibraryWebsite.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            await using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminId = await EnsureUser(serviceProvider, testUserPw, "superAdmin", "admin@contoso.com");
                await EnsureRole(serviceProvider, adminId, Role.Admin.ToString());
                await context.Readers.AddAsync(new Reader
                {
                    ReaderId = adminId,
                    FirstName = "James",
                    LastName = "Smith",
                });
                await context.SaveChangesAsync();

                var librarianId = await EnsureUser(serviceProvider, testUserPw, "justALibrarian", "librarian@contoso.com");
                await EnsureRole(serviceProvider, librarianId, Role.Librarian.ToString());

                await SeedDb(context, serviceProvider);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
            string testUserPw, string userName, string email)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = email,
                    FirstName = "James",
                    LastName = "Smith",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
            string uid, string role)
        {
            IdentityResult ir;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }

        private static async Task<string> CreateUser(IServiceProvider serviceProvider, User user, string password)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            await userManager.CreateAsync(user, password);

            return user.Id;
        }

        private static async Task<string> SeedDb(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            if (context.Books.Any())
            {
                return string.Empty;
            }

            var books = new[]
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

            foreach (var i in new List<int> { 0, 3, 5, 6 })
            {
                books[i].IsBorrowed = true;
            }

            foreach (Book b in books)
            {
                context.Books.Add(b);
            }

            var readers = new[]
            {
                new Reader{FirstName = "John", LastName = "Smith", NumberOfLoans = 1},
                new Reader{FirstName = "Jimmy", LastName = "Johnson", NumberOfRequests = 1, NumberOfLoans = 1},
                new Reader{FirstName = "Emily", LastName = "Richardson", NumberOfRequests = 1},
                new Reader{FirstName = "Elisabeth", LastName = "Lee", NumberOfLoans = 2}
            };

            User[] users = new User[readers.Length];

            foreach (Reader r in readers)
            {
                context.Readers.Add(r);
            }

            await context.SaveChangesAsync();

            for (int i = 0; i < users.Length; i++)
            {
                users[i] = new User
                {
                    UserName = readers[i].FirstName + readers[i].LastName,
                    Email = "me" + i + "@gmail.com",
                    FirstName = readers[i].FirstName,
                    LastName = readers[i].LastName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Id = readers[i].ReaderId
                };

                string userId = await CreateUser(serviceProvider, users[i], "123Pa$$word.");
                await EnsureRole(serviceProvider, userId, Role.Reader.ToString());
            }

            var loans = new[]
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

            var requests = new[]
            {
                new Request(readers[1].ReaderId, "For Whom the Bell Tolls", "Ernest Hemingway", Genres.Novel),
                new Request(readers[2].ReaderId, "The Hobbit, or There and Back Again", "J.R.R. Tolkien", Genres.Fantasy)
            };

            foreach (Request r in requests)
            {
                context.Requests.Add(r);
            }

            await context.SaveChangesAsync();

            return string.Empty;
        }
    }
}
