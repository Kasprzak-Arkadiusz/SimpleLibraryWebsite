using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Services.Authorization;

namespace SimpleLibraryWebsite.Data
{
    public class DbInitializer
    {
        private readonly IServiceProvider _serviceProvider;

        public DbInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Initialize()
        {
            await using var context = new ApplicationDbContext(
                _serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            await CreateAdminAccount(context, "superAdmin", "admin@gmail.com", "James", "Smith");
            await CreateLibrarianAccount("justALibrarian", "librarian@gmail.com", "John", "Williams");

            await SeedDb(context);
        }

        private async Task CreateAdminAccount(ApplicationDbContext context,
            string userName, string email, string firstName, string lastName)
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();

            User user = await CreateUser(userName, email, firstName, lastName, config["AdminSeedPassword"]);

            Reader reader = new(user);
            context.Readers.Add(reader);
            await context.SaveChangesAsync();

            await EnsureRole(user.Id, Role.Admin.ToString());
            await EnsureRole(user.Id, Role.Reader.ToString());
        }

        private async Task CreateLibrarianAccount(string userName, string email, string firstName, string lastName)
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();

            User user = await CreateUser(userName, email, firstName, lastName, config["LibrarianSeedPassword"]);

            await EnsureRole(user.Id, Role.Librarian.ToString());
        }

        private async Task<User> CreateUser(string userName, string email, string firstName, string lastName, string password)
        {
            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            User user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await userManager.CreateAsync(user, password);
            }

            if (user == null)
            {
                throw new Exception("User couldn't be created. The password was probably not strong enough!");
            }

            return user;
        }

        private async Task EnsureRole(string id, string role)
        {
            var roleManager = _serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("RoleManager instance is null.");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            User user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new Exception("Couldn't find a user with provided id.");
            }

            await userManager.AddToRoleAsync(user, role);
        }

        private async Task<User> CreateUser(User user, string password)
        {
            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            await userManager.CreateAsync(user, password);

            return user;
        }

        private async Task SeedDb(ApplicationDbContext context)
        {
            if (context.Books.Any())
            {
                return;
            }

            var books = new[]
            {
                new Book("Adam Mickiewicz", "Master Thaddeus",Genres.Poetry),
                new Book("Adam Mickiewicz","Ode to Youth", Genres.Poetry),
                new Book("Stephen King",  "It",  Genres.Horror),
                new Book("Jules Verne", "Twenty Thousand Leagues Under the Seas", Genres.Adventure),
                new Book("Herman Melville","Moby Dick", Genres.Novel),
                new Book("George Orwell", "Nineteen Eighty-Four", Genres.SciFi),
                new Book("J.R.R. Tolkien", "Lord of the Rings", Genres.Fantasy),
                new Book("Jane Austen", "Pride and Prejudice", Genres.Romance),
                new Book("Walter Isaacson", "Steve Jobs", Genres.Biography)
            };

            foreach (int i in new List<int> { 0, 3, 5, 6 })
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

            var users = new User[readers.Length];

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

                User user = await CreateUser(users[i], "123Pa$$word.");
                await EnsureRole(user.Id, Role.Reader.ToString());
            }

            var loans = new[]
            {
                new Loan(1, readers[0].ReaderId,  DateTime.Now),
                new Loan(4, readers[1].ReaderId, DateTime.Parse("01/02/2021")),
                new Loan(6, readers[3].ReaderId, DateTime.Parse("15/04/2021")),
                new Loan(7, readers[3].ReaderId, DateTime.Parse("16/05/2021"))
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
        }
    }
}
