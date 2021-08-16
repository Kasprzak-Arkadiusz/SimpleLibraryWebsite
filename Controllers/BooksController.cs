using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;

namespace SimpleLibraryWebsite.Controllers
{
    public class BooksController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public BooksController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<User> userManager)
            : base(context, authorizationService, userManager)
        { }

        // GET: Books
        [AllowAnonymous]
        public async Task<IActionResult> Index(string bookGenre, string bookTitle, string sortOrder,
                                                string currentGenreFilter, string currentTitleFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewData["CurrentGenreFilter"] = SaveFilterValue(ref bookGenre, currentGenreFilter, ref pageNumber);

            var books = from b in Context.Books select b;

            if (!string.IsNullOrEmpty(bookTitle))
            {
                books = books.Where(b => b.Title.Contains(bookTitle));
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                _ = Enum.TryParse(bookGenre, out Genres genre);
                books = books.Where(b => b.Genre == genre);
            }

            books = sortOrder switch
            {
                "title_desc" => books.OrderByDescending(b => b.Title),
                "Author" => books.OrderBy(b => b.Author),
                "author_desc" => books.OrderByDescending(b => b.Author),
                _ => books.OrderBy(b => b.Title)
            };

            IQueryable<string> stringGenres = from b in Context.Books
                                              orderby b.Genre
                                              select b.Genre.ToString();
            const int pageSize = 5;

            BookGenreViewModel bookGenreViewModel = new BookGenreViewModel
            {
                Genres = new SelectList(await stringGenres.Distinct().ToListAsync()),
                PaginatedList = await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize)
            };

            return View(bookGenreViewModel);
        }

        // GET: Books/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await Context.Books.AsNoTracking()
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Borrow/5
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Borrow(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await Context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            BookBorrowViewModel bookBorrowViewModel = new(book);

            return View(bookBorrowViewModel);
        }

        
        // POST: Books/BorrowPost/5
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        [HttpPost, ActionName(nameof(Borrow))]
        public async Task<IActionResult> BorrowPost(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null || userId == null)
            {
                return NotFound();
            }

            Reader borrowingReader = await Context.Readers.SingleOrDefaultAsync(r => r.ReaderId == userId);
            Book borrowedBook = await Context.Books.SingleOrDefaultAsync(b => b.BookId == id);

            if (borrowingReader.NumberOfLoans == Reader.BookLoansLimit)
            {
                ModelState.AddModelError("", "You have exceeded the limit of borrowed books.\n "
                                                        + $"You can borrow a maximum of {Reader.BookLoansLimit} books.\n"
                                                        + "Return some books before borrowing a new book");
                return View(new BookBorrowViewModel(borrowedBook));
            }

            if (borrowedBook.IsBorrowed)
            {
                ModelState.AddModelError("", "This book is already on loan. Try another time.");

                return View(new BookBorrowViewModel(borrowedBook));
            }

            borrowedBook.IsBorrowed = true;
            if (await TryUpdateModelAsync(
                borrowedBook,
                "",
                b => b.IsBorrowed))
            {
                try
                {
                    Loan loan = new(id.GetValueOrDefault(), userId, DateTime.Today);
                    Context.Loans.Add(loan);
                    borrowingReader.NumberOfLoans++;
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.Error(ex);
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists " +
                                                 "see your system administrator.");
                    return View(new BookBorrowViewModel(borrowedBook));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Create
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Create([Bind("Author,Title,Genre")] Book book)
        {
            if (book.AnyFieldIsNullOrEmpty())
            {
                ModelState.AddModelError("", "All fields must be filled");
                return View(book);
            }

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

        // GET: Books/Edit/5
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await Context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Edit/5
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        [HttpPost, ActionName(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var bookToUpdate = await Context.Books.FirstOrDefaultAsync(b => b.BookId == id);

            if (await TryUpdateModelAsync(
                bookToUpdate,
                "",
                b => b.Title, b => b.Author, b => b.Genre))
            {
                try
                {
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.Error(ex);
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists " +
                                                 "see your system administrator.");
                }
            }

            return View(bookToUpdate);
        }

        // GET: Books/Delete/5
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

        // POST: Books/Delete/5
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
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
