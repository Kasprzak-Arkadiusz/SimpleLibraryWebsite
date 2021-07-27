using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index(string readerName, string readerSurname, string bookTitle, string sortOrder)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["ReaderNameSortParam"] = sortOrder == "ReaderName" ? "readerName_desc" : "ReaderName";
            ViewData["ReaderSurnameSortParam"] = sortOrder == "ReaderSurname" ? "readerSurname_desc" : "ReaderSurname";

            IQueryable<Loan> loans = _context.Loans.Include(l => l.Reader).Include(l => l.Book);

            var readers = from r in _context.Readers select r;
            var books = from b in _context.Books select b;
            bool isAnySearchFieldFilled = false;

            if (!string.IsNullOrWhiteSpace(readerName))
            {
                readers = from r in readers where r.Name == readerName select r;
                isAnySearchFieldFilled = true;
            }

            if (!string.IsNullOrWhiteSpace(readerSurname))
            {
                readers = from r in readers where r.Surname == readerSurname select r;
                isAnySearchFieldFilled = true;
            }

            if (!readers.Any())
            {
                return View(new LoanViewModel() { Loans = new List<Loan>() });
            }

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                books = from b in books where b.Title.Contains(bookTitle) select b;
                isAnySearchFieldFilled = true;
            }

            LoanViewModel loanViewModel = new LoanViewModel();

            if (isAnySearchFieldFilled)
            {
                var readersList = await readers.ToListAsync();
                var booksList = await books.ToListAsync();
                var loansList = await loans.ToListAsync();
                var result = loansList
                    .Where(l => readersList.Any(read => read.ReaderID == l.ReaderID) &&
                                booksList.Any(book => book.BookID == l.BookID))
                    .OrderBy(l => l.Book.Title);
                loanViewModel.Loans = result.ToList();
            }
            else
            {
                loanViewModel.Loans = await loans.ToListAsync();
            }

            loanViewModel.Loans = sortOrder switch
            {
                "title_desc" => loanViewModel.Loans.OrderByDescending(l => l.Book.Title).ToList(),
                "ReaderName" => loanViewModel.Loans.OrderBy(l => l.Reader.Name).ToList(),
                "readerName_desc" => loanViewModel.Loans.OrderByDescending(l => l.Reader.Name).ToList(),
                "ReaderSurname" => loanViewModel.Loans.OrderBy(l => l.Reader.Surname).ToList(),
                "readerSurname_desc" => loanViewModel.Loans.OrderByDescending(l => l.Reader.Surname).ToList(),
                _ => loanViewModel.Loans
            };

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
                .FirstOrDefaultAsync(m => m.LoanID == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID");
            ViewData["BookID"] = new SelectList(_context.Books, "BookID", "BookID");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanID,BookID,ReaderID,LentFrom,LentTo")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                loan.FillMissingFields();
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID", loan.ReaderID);
            ViewData["BookID"] = new SelectList(_context.Books, "BookID", "BookID", loan.BookID);
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
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID", loan.ReaderID);
            ViewData["BookID"] = new SelectList(_context.Books, "BookID", "BookID", loan.BookID);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanID,BookID,ReaderID,LentFrom,LentTo")] Loan loan)
        {
            if (id != loan.LoanID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.LoanID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID", loan.ReaderID);
            ViewData["BookID"] = new SelectList(_context.Books, "BookID", "BookID", loan.BookID);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .FirstOrDefaultAsync(m => m.LoanID == id);
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
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanID == id);
        }
    }
}
