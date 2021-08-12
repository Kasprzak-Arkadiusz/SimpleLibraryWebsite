using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;

namespace SimpleLibraryWebsite.Controllers
{
    public class LoansController : CustomController
    {
        private readonly ApplicationDbContext _context;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index(string readerName, string readerLastName, string bookTitle, string sortOrder,
                                                string currentNameFilter, string currentLastNameFilter, string currentTitleFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["ReaderNameSortParam"] = sortOrder == "ReaderName" ? "readerName_desc" : "ReaderName";
            ViewData["ReaderLastNameSortParam"] = sortOrder == "ReaderLastName" ? "readerLastName_desc" : "ReaderLastName";
            ViewData["LentToSortParam"] = sortOrder == "LentTo" ? "lentTo_desc" : "LentTo";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentNameFilter"] = SaveFilterValue(ref readerName, currentNameFilter, ref pageNumber);
            ViewData["CurrentLastNameFilter"] = SaveFilterValue(ref readerLastName, currentLastNameFilter, ref pageNumber);
            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);

            var readers = from r in _context.Readers select r;
            bool isAnySearchFieldFilled = false;
            if (!string.IsNullOrWhiteSpace(readerName))
            {
                readers = from r in readers where r.FirstName == readerName select r;
                isAnySearchFieldFilled = true;
            }

            if (!string.IsNullOrWhiteSpace(readerLastName))
            {
                readers = from r in readers where r.LastName == readerLastName select r;
                isAnySearchFieldFilled = true;
            }

            if (!readers.Any())
            {
                return View(new LoanViewModel { PaginatedList = new PaginatedList<Loan>() });
            }

            var books = from b in _context.Books select b;
            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                books = from b in books where b.Title.Contains(bookTitle) select b;
                isAnySearchFieldFilled = true;
            }

            LoanViewModel loanViewModel = new LoanViewModel();
            IQueryable<Loan> loans = _context.Loans.Include(l => l.Reader).Include(l => l.Book);
            if (isAnySearchFieldFilled)
            {
                loanViewModel.Loans = loans
                    .Where(l => readers.Any(read => read.ReaderId == l.ReaderId) &&
                                books.Any(book => book.BookId == l.BookId)).ToList();
            }
            else
            {
                loanViewModel.Loans = await loans.ToListAsync();
            }

            var results = sortOrder switch
            {
                "title_desc" => loanViewModel.Loans.OrderByDescending(l => l.Book.Title),
                "ReaderName" => loanViewModel.Loans.OrderBy(l => l.Reader.FirstName),
                "readerName_desc" => loanViewModel.Loans.OrderByDescending(l => l.Reader.FirstName),
                "ReaderLastName" => loanViewModel.Loans.OrderBy(l => l.Reader.LastName),
                "readerLastName_desc" => loanViewModel.Loans.OrderByDescending(l => l.Reader.LastName),
                "LentTo" => loanViewModel.Loans.OrderBy(l => l.LentTo),
                "lentTo_desc" => loanViewModel.Loans.OrderByDescending(l => l.LentTo),
                _ => loanViewModel.Loans.OrderBy(l => l.Book.Title)
            };
            loanViewModel.Loans = results.ToList();

            const int pageSize = 2;
            loanViewModel.PaginatedList = PaginatedList<Loan>.Create(loanViewModel.Loans, pageNumber ?? 1, pageSize);

            return View(loanViewModel);
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(r => r.Reader).AsNoTracking()
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Return/5
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            Book returnedBook = await _context.Books.SingleOrDefaultAsync(b => b.BookId == loan.BookId);
            Reader reader = await _context.Readers.SingleOrDefaultAsync(b => b.ReaderId == loan.ReaderId);
            BookReturnViewModel bookReturnViewModel = new(loan, returnedBook, reader);

            return View(bookReturnViewModel);
        }

        // POST: Books/Return/5
        [HttpPost, ActionName("Return")]
        public async Task<IActionResult> ReturnPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan loan = await _context.Loans.SingleOrDefaultAsync(l => l.LoanId == id);
            Book returnedBook = await _context.Books.SingleOrDefaultAsync(b => b.BookId == loan.BookId);
            Reader reader = await _context.Readers.SingleOrDefaultAsync(r => r.ReaderId == loan.ReaderId);
            BookReturnViewModel bookReturnViewModel = new(loan, returnedBook, reader);

            returnedBook.IsBorrowed = false;
            if (await TryUpdateModelAsync(
                returnedBook,
                "",
                b => b.IsBorrowed))
            {
                try
                {
                    _context.Loans.Remove(loan);
                    reader.NumberOfLoans--;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.Error(ex);
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists " +
                                                 "see your system administrator.");
                    return View(bookReturnViewModel);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private void CreateBookIdList()
        {
            var bookIdList = (from b in _context.Books
                select new SelectListItem()
                {
                    Text = b.BookId.ToString(),
                    Value = b.BookId.ToString(),
                }).ToList();

            ViewBag.ListOfBookId = bookIdList;
        }

        private void CreateReaderIdList()
        {
            var readerIdList = (from r in _context.Readers
                select new SelectListItem()
                {
                    Text = r.ReaderId.ToString(),
                    Value = r.ReaderId.ToString(),
                }).ToList();

            ViewBag.ListOfReaderId = readerIdList;
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            CreateBookIdList();
            CreateReaderIdList();

            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,ReaderId,LentFrom,LentTo")] Loan loan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    loan.FillMissingFields();
                    _context.Add(loan);
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

            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            CreateReaderIdList();
            CreateBookIdList();

            return View(loan);
        }

        // POST: Loans/Edit/5
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

            var loanToUpdate = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == id);

            if (await TryUpdateModelAsync(
                loanToUpdate,
                "",
                l => l.ReaderId, l => l.BookId, l => l.LentTo))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.Error(ex.Message);
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists " +
                                                 "see your system administrator.");
                }
            }

            return View(loanToUpdate);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message);
                return RedirectToAction(nameof(Delete), new {id, saveChangesError = true});
            }
        }
    }
}
