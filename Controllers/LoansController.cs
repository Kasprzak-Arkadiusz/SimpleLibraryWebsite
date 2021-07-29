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
        public async Task<IActionResult> Index(string readerName, string readerSurname, string bookTitle, string sortOrder,
                                                string currentNameFilter, string currentSurnameFilter, string currentTitleFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["ReaderNameSortParam"] = sortOrder == "ReaderName" ? "readerName_desc" : "ReaderName";
            ViewData["ReaderSurnameSortParam"] = sortOrder == "ReaderSurname" ? "readerSurname_desc" : "ReaderSurname";
            ViewData["LentToSortParam"] = sortOrder == "LentTo" ? "lentTo_desc" : "LentTo";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentNameFilter"] = SaveFilterValue(ref readerName, currentNameFilter, ref pageNumber);
            ViewData["CurrentSurnameFilter"] = SaveFilterValue(ref readerSurname, currentSurnameFilter, ref pageNumber);
            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);

            var readers = from r in _context.Readers select r;
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
                return View(new LoanViewModel() { PaginatedList = new PaginatedList<Loan>() });
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
                    .Where(l => readers.Any(read => read.ReaderID == l.ReaderID) &&
                                books.Any(book => book.BookID == l.BookID)).ToList();
            }
            else
            {
                loanViewModel.Loans = await loans.ToListAsync();
            }

            var results = sortOrder switch
            {
                "title_desc" => loanViewModel.Loans.OrderByDescending(l => l.Book.Title),
                "ReaderName" => loanViewModel.Loans.OrderBy(l => l.Reader.Name),
                "readerName_desc" => loanViewModel.Loans.OrderByDescending(l => l.Reader.Name),
                "ReaderSurname" => loanViewModel.Loans.OrderBy(l => l.Reader.Surname),
                "readerSurname_desc" => loanViewModel.Loans.OrderByDescending(l => l.Reader.Surname),
                "LentTo" => loanViewModel.Loans.OrderBy(l => l.LentTo),
                "lentTo_desc" => loanViewModel.Loans.OrderByDescending(l => l.LentTo),
                _ => loanViewModel.Loans.OrderBy(l => l.Book.Title)
            };
            loanViewModel.Loans = results.ToList();

            const int pageSize = 2;
            loanViewModel.PaginatedList = PaginatedList<Loan>.Create(loanViewModel.Loans, pageNumber ?? 1, pageSize);

            return View(loanViewModel);
        }

        private string SaveFilterValue(ref string value, string valueToSave, ref int? pageNumber)
        {
            if (value is not null)
            {
               pageNumber = 1;
            }

            return value ??= valueToSave;
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
