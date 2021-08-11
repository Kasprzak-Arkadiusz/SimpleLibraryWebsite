﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class BooksController : CustomController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public BooksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index(string bookGenre, string bookTitle, string sortOrder,
                                                string currentGenreFilter, string currentTitleFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewData["CurrentGenreFilter"] = SaveFilterValue(ref bookGenre, currentGenreFilter, ref pageNumber);

            var books = from b in _context.Books select b;

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

            IQueryable<string> stringGenres = from b in _context.Books
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.AsNoTracking()
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Borrow/5
        public async Task<IActionResult> Borrow(int? bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            if (bookId == null || userId == null)
            {
                return NotFound();
            }

            Loan loan = new Loan(bookId.GetValueOrDefault(), userId , DateTime.Today);
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Author,Title,Genre")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    book.FillMissingProperties();
                    _context.Add(book);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);

            if (await TryUpdateModelAsync(
                bookToUpdate,
                "",
                b => b.Title, b => b.Author, b => b.Genre))
            {
                try
                {
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
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
