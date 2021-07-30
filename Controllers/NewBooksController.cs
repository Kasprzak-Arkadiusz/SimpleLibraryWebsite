using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class NewBooksController : CustomController
    {
        private readonly ApplicationDbContext _context;

        public NewBooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NewBooks
        public async Task<IActionResult> Index(string bookGenre, string bookTitle, string sortOrder,
                                                string currentGenreFilter, string currentTitleFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewData["CurrentGenreFilter"] = SaveFilterValue(ref bookGenre, currentGenreFilter, ref pageNumber);

            var newBooks = from b in _context.Books select b;

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

            var newBooksList = newBooks.Where(b => b.AddingDate.Date >= today - borrowingTime).ToList();

            var results = sortOrder switch
            {
                "title_desc" => newBooksList.OrderByDescending(b => b.Title),
                "Author" => newBooksList.OrderBy(b => b.Author),
                "author_desc" => newBooksList.OrderByDescending(b => b.Author),
                _ => newBooksList.OrderBy(b => b.Title)
            };

            IQueryable<string> stringGenres = from b in _context.Books
                                              orderby b.Genre
                                              select b.Genre.ToString();
            const int pageSize = 5;

            BookGenreViewModel bookGenreViewModel = new BookGenreViewModel
            {
                Genres = new SelectList(await stringGenres.Distinct().ToListAsync()),
                PaginatedList = PaginatedList<Book>.Create(results, pageNumber ?? 1, pageSize)
            };

            return View(bookGenreViewModel);
        }

        // GET: NewBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: NewBooks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookID,Author,Title,Genre,AddingDate,IsBorrowed")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: NewBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: NewBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
