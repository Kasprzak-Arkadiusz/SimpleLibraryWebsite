using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;

namespace SimpleLibraryWebsite.Controllers
{
    public class LoansController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public LoansController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<User> userManager)
            : base(context, authorizationService, userManager)
        { }

        // GET: Loans
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
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

            var readers = from r in Context.Readers select r;
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

            var books = from b in Context.Books select b;
            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                books = from b in books where b.Title.Contains(bookTitle) select b;
                isAnySearchFieldFilled = true;
            }

            LoanViewModel loanViewModel = new LoanViewModel();
            IQueryable<Loan> loans = Context.Loans.Include(l => l.Reader).Include(l => l.Book);
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
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await Context.Loans
                .Include(r => r.Reader).AsNoTracking()
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Return/5
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await Context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            Book returnedBook = await Context.Books.SingleOrDefaultAsync(b => b.BookId == loan.BookId);
            Reader reader = await Context.Readers.SingleOrDefaultAsync(b => b.ReaderId == loan.ReaderId);
            BookReturnViewModel bookReturnViewModel = new(loan, returnedBook, reader);

            return View(bookReturnViewModel);
        }

        // POST: Books/Return/5
        [HttpPost, ActionName(nameof(Return))]
        [AuthorizeEnum(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> ReturnPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan loan = await Context.Loans.SingleOrDefaultAsync(l => l.LoanId == id);
            Book returnedBook = await Context.Books.SingleOrDefaultAsync(b => b.BookId == loan.BookId);
            Reader reader = await Context.Readers.SingleOrDefaultAsync(r => r.ReaderId == loan.ReaderId);
            BookReturnViewModel bookReturnViewModel = new(loan, returnedBook, reader);

            returnedBook.IsBorrowed = false;
            if (await TryUpdateModelAsync(
                returnedBook,
                "",
                b => b.IsBorrowed))
            {
                try
                {
                    Context.Loans.Remove(loan);
                    reader.NumberOfLoans--;
                    await Context.SaveChangesAsync();
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
    }
}
