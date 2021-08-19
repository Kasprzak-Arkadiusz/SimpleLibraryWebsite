using System;
using System.Globalization;
using System.Linq;
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
    public class NewBooksController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly UnitOfWork _unitOfWork;

        public NewBooksController(ApplicationDbContext context)

        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: NewBooks
        [AllowAnonymous]
        public async Task<IActionResult> Index(string bookGenre, string bookTitle, string sortOrder,
                                                string currentGenreFilter, string currentTitleFilter, int? pageNumber)
        {
            SaveCurrentSortingAndFiltering(ref bookGenre, ref bookTitle, sortOrder,
                currentGenreFilter, currentTitleFilter, ref pageNumber);

            var newBooks =  _unitOfWork.BookRepository.Get();

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

            newBooks = newBooks.Where(b => b.DateOfAdding.Date >= today - borrowingTime);

            newBooks = SortBooks(newBooks, sortOrder);

            var stringGenres = _unitOfWork.BookRepository.Get()
                .OrderBy(b => b.Genre)
                .Select(b => b.Genre.ToString());

            const int pageSize = 5;

            BookGenreViewModel bookGenreViewModel = new ()
            {
                Genres = new SelectList(await stringGenres.Distinct().ToListAsync()),
                PaginatedList = await newBooks.ToPagedListAsync(pageNumber ?? 1, pageSize)
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

        // GET: NewBooks/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id.Value);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: NewBooks/Create
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewBooks/Create
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
                    book.FillMissingProperties();
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

        // GET: NewBooks/Delete/5
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

        // POST: NewBooks/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Book book = await _unitOfWork.BookRepository.GetByIdAsync(id);

            if (book is null)
            {
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }
    }
}
