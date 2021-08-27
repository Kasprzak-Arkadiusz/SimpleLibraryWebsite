using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Data.DAL;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;
using X.PagedList;

namespace SimpleLibraryWebsite.Controllers
{
    public class BooksController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly UnitOfWork _unitOfWork;

        public BooksController(ApplicationDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: Books
        [AllowAnonymous]
        public async Task<IActionResult> Index(string bookGenre, string bookTitle, string sortOrder,
            string currentGenreFilter, string currentTitleFilter, int? pageNumber, bool isNewBooksView)
        {
            SaveCurrentSortingAndFiltering(ref bookGenre, ref bookTitle, sortOrder,
                currentGenreFilter, currentTitleFilter, ref pageNumber);

            var books = _unitOfWork.BookRepository.Get();

            if (!string.IsNullOrEmpty(bookTitle))
            {
                books = books.Where(b => b.Title.Contains(bookTitle));
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                _ = Enum.TryParse(bookGenre, out Genres genre);
                books = books.Where(b => b.Genre == genre);
            }

            books = SortBooks(books, sortOrder);

            var stringGenres = _unitOfWork.BookRepository.Get()
                .OrderBy(b => b.Genre)
                .Select(b => b.Genre.ToString());

            const int pageSize = 5;

            BookGenreViewModel bookGenreViewModel = new()
            {
                Genres = new SelectList(await stringGenres.Distinct().ToListAsync()),
                PaginatedList = await books.ToPagedListAsync(pageNumber ?? 1, pageSize)
            };

            return View(bookGenreViewModel);
        }

        // GET: Books
        [AllowAnonymous]
        public async Task<IActionResult> NewBooksIndex(string bookGenre, string bookTitle, string sortOrder,
            string currentGenreFilter, string currentTitleFilter, int? pageNumber)
        {
            SaveCurrentSortingAndFiltering(ref bookGenre, ref bookTitle, sortOrder,
                currentGenreFilter, currentTitleFilter, ref pageNumber);

            var culture = CultureInfo.CreateSpecificCulture("en-US");

            DateTime.TryParse(DateTime.Today.ToString(culture), culture, DateTimeStyles.None, out DateTime today);
            TimeSpan.TryParse(TimeSpan.FromDays(Book.DaysToStopBeingANewBook).ToString(), out TimeSpan borrowingTime);

            var books = _unitOfWork.BookRepository.Get(b => b.DateOfAdding.Date >= today - borrowingTime);

            if (!string.IsNullOrEmpty(bookTitle))
            {
                books = books.Where(b => b.Title.Contains(bookTitle));
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                _ = Enum.TryParse(bookGenre, out Genres genre);
                books = books.Where(b => b.Genre == genre);
            }

            books = SortBooks(books, sortOrder);

            var stringGenres = _unitOfWork.BookRepository.Get()
                .OrderBy(b => b.Genre)
                .Select(b => b.Genre.ToString());

            const int pageSize = 5;

            BookGenreViewModel bookGenreViewModel = new()
            {
                Genres = new SelectList(await stringGenres.Distinct().ToListAsync()),
                PaginatedList = await books.ToPagedListAsync(pageNumber ?? 1, pageSize)
            };

            return View(bookGenreViewModel);
        }

        private void SaveCurrentSortingAndFiltering(ref string bookGenre, ref string bookTitle, string sortOrder,
            string currentGenreFilter, string currentTitleFilter, ref int? pageNumber)
        {
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.AuthorSortParam = sortOrder == "Author" ? "author_desc" : "Author";
            ViewBag.CurrentSort = sortOrder;

            ViewBag.CurrentTitleFilter = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewBag.CurrentGenreFilter = SaveFilterValue(ref bookGenre, currentGenreFilter, ref pageNumber);
        }

        private IQueryable<Book> SortBooks(IQueryable<Book> books, string sortOrder)
        {
            return sortOrder switch
            {
                "title_desc" => books.OrderByDescending(b => b.Title),
                "Author" => books.OrderBy(b => b.Author),
                "author_desc" => books.OrderByDescending(b => b.Author),
                _ => books.OrderBy(b => b.Title)
            };
        }

        // GET: Books/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Borrow/5
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Borrow(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(new BookBorrowViewModel(book));
        }


        // POST: Books/BorrowPost/5
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        [HttpPost, ActionName(nameof(Borrow))]
        public async Task<IActionResult> BorrowPost(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null || userId == null)
            {
                return NotFound();
            }

            Reader borrowingReader = await _unitOfWork.ReaderRepository.GetByIdAsync(userId);
            Book borrowedBook = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (borrowingReader.NumberOfLoans >= Reader.BookLoansLimit)
            {
                ModelState.AddModelError("", "You have exceeded the limit of borrowed books.\n "
                                             + $"You can borrow a maximum of {Reader.BookLoansLimit} books.\n"
                                             + "Return some books before borrowing a new book.");
                return View(new BookBorrowViewModel(borrowedBook));
            }

            if (borrowedBook.IsBorrowed)
            {
                ModelState.AddModelError("", "This book is already on loan. Try another time.");

                return View(new BookBorrowViewModel(borrowedBook));
            }

            try
            {
                borrowedBook.IsBorrowed = true;

                await _unitOfWork.LoanRepository.InsertAsync(new Loan(id.Value, userId));
                borrowingReader.NumberOfLoans++;
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex, "DB exception");
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
                return View(new BookBorrowViewModel(borrowedBook));
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Create
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Create([Bind("Author,Title,Genre")] Book book)
        {
            if (book.AnyFieldIsNullOrEmpty())
            {
                ModelState.AddModelError("", "All fields must be filled.");
                return View(book);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    book.Author = book.Author.Trim();
                    book.Title = book.Title.Trim();
                    await _unitOfWork.BookRepository.InsertAsync(book);
                    await _unitOfWork.SaveAsync();
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
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Edit/5
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        [HttpPost, ActionName(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id is null)
            {
                return NotFound();
            }

            Book bookToUpdate = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (bookToUpdate == null)
            {
                Book deletedBook = new();
                await TryUpdateModelAsync(deletedBook);
                ModelState.AddModelError(string.Empty, "Unable to save changes. The book was deleted by another user.");
                return View(deletedBook);
            }

            _unitOfWork.BookRepository.SetRowVersionOriginalValue(bookToUpdate, rowVersion);

            if (await TryUpdateModelAsync(
                bookToUpdate,
                "",
                b => b.Title, b => b.Author, b => b.Genre))
            {
                try
                {
                    bookToUpdate.Author = bookToUpdate.Author.Trim();
                    bookToUpdate.Title = bookToUpdate.Title.Trim();

                    await _unitOfWork.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Book)exceptionEntry.Entity;
                    var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The book was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Book)databaseEntry.ToObject();

                        if (databaseValues.Author != clientValues.Author)
                        {
                            ModelState.AddModelError(nameof(databaseValues.Author),
                                $"Current value: {databaseValues.Author}");
                        }

                        if (databaseValues.Title != clientValues.Title)
                        {
                            ModelState.AddModelError(nameof(databaseValues.Title),
                                $"Current value: {databaseValues.Title}");
                        }

                        if (databaseValues.Genre != clientValues.Genre)
                        {
                            ModelState.AddModelError(nameof(databaseValues.Genre),
                                $"Current value: {databaseValues.Genre}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                                               + "was modified by another user after you got the original value. The "
                                                               + "edit operation was canceled and the current values in the database "
                                                               + "have been displayed. If you still want to edit this record, click "
                                                               + "the Save button again. Otherwise click the Back hyperlink.");
                        bookToUpdate.RowVersion = databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            return View(bookToUpdate);
        }

        // GET: Books/Delete/5
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (book is null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (book.IsBorrowed)
            {
                ModelState.AddModelError(String.Empty, "The book you attempted to delete is borrowed by some user.");
                return View(book);
            }

            try
            {
                _unitOfWork.BookRepository.Delete(book);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message);
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
