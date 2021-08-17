using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;
using X.PagedList;

namespace SimpleLibraryWebsite.Controllers
{
    public class NewBooksController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public NewBooksController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<User> userManager)
            : base(context, authorizationService, userManager)
        { }

        // GET: NewBooks
        [AllowAnonymous]
        public async Task<IActionResult> Index(string bookGenre, string bookTitle, string sortOrder,
                                                string currentGenreFilter, string currentTitleFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewData["CurrentGenreFilter"] = SaveFilterValue(ref bookGenre, currentGenreFilter, ref pageNumber);

            var newBooks = from b in Context.Books select b;

            if (!string.IsNullOrEmpty(bookTitle))
            {
                newBooks = newBooks.Where(b => b.Title.Contains(bookTitle));
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                _ = Enum.TryParse(bookGenre, out Genres genre);
                newBooks = newBooks.Where(b => b.Genre == genre);
            }

            var culture = CultureInfo.CreateSpecificCulture("en-US");

            DateTime.TryParse(DateTime.Today.ToString(culture), culture, DateTimeStyles.None, out DateTime today);
            TimeSpan.TryParse(TimeSpan.FromDays(14).ToString(), out TimeSpan borrowingTime);

            var newBooksList = newBooks.Where(b => b.DateOfAdding.Date >= today - borrowingTime).ToList();

            var results = sortOrder switch
            {
                "title_desc" => newBooksList.OrderByDescending(b => b.Title),
                "Author" => newBooksList.OrderBy(b => b.Author),
                "author_desc" => newBooksList.OrderByDescending(b => b.Author),
                _ => newBooksList.OrderBy(b => b.Title)
            };

            IQueryable<string> stringGenres = from b in Context.Books
                                              orderby b.Genre
                                              select b.Genre.ToString();
            const int pageSize = 5;

            BookGenreViewModel bookGenreViewModel = new BookGenreViewModel
            {
                Genres = new SelectList(await stringGenres.Distinct().ToListAsync()),
                PaginatedList = results.ToPagedList(pageNumber ?? 1, pageSize)
            };

            return View(bookGenreViewModel);
        }

        // GET: NewBooks/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await Context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: NewBooks/Create
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Create([Bind("Author,Title,Genre")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    book.FillMissingProperties();
                    Context.Add(book);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }
            
            return View(book);
        }

        // GET: NewBooks/Delete/5
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await Context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: NewBooks/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await Context.Books.FindAsync(id);
            if (book is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Context.Books.Remove(book);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message);
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }
    }
}
