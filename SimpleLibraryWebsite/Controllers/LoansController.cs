using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Data.DAL;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;
using SimpleLibraryWebsite.Services.Authorization;
using X.PagedList;

namespace SimpleLibraryWebsite.Controllers
{
    public class LoansController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly UnitOfWork _unitOfWork;

        public LoansController(ApplicationDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: Loans
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Index(string readerFirstName, string readerLastName, string bookTitle, string sortOrder,
                                                string currentFirstNameFilter, string currentLastNameFilter,
                                                string currentTitleFilter, int? pageNumber)
        {
            SaveCurrentSortingAndFiltering(ref readerFirstName, ref readerLastName, bookTitle, sortOrder,
                 currentFirstNameFilter, currentLastNameFilter, currentTitleFilter, ref pageNumber);

            var readers = _unitOfWork.ReaderRepository.Get();
            bool isAnySearchFieldFilled = false;

            if (!string.IsNullOrWhiteSpace(readerFirstName))
            {
                readers = _unitOfWork.ReaderRepository.Get(r => r.FirstName == readerFirstName);
                isAnySearchFieldFilled = true;
            }
            if (!string.IsNullOrWhiteSpace(readerLastName))
            {
                readers = _unitOfWork.ReaderRepository.Get(r => r.LastName == readerLastName);
                isAnySearchFieldFilled = true;
            }

            var books = _unitOfWork.BookRepository.Get();

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                books = _unitOfWork.BookRepository.Get(b => b.Title.Contains(bookTitle));
                isAnySearchFieldFilled = true;
            }

            var loans =
                _unitOfWork.LoanRepository.Get(includeProperties: nameof(Loan.Reader) + "," + nameof(Loan.Book));

            if (isAnySearchFieldFilled)
            {
                loans = loans
                    .Where(l =>
                        readers.Any(r => r.ReaderId == l.ReaderId) &&
                        books.Any(b => b.BookId == l.BookId));
            }

            loans = SortLoans(loans, sortOrder);

            const int pageSize = 2;

            LoanViewModel loanViewModel = new()
            {
                PaginatedList = await loans.ToPagedListAsync(pageNumber ?? 1, pageSize)
            };

            return View(loanViewModel);
        }

        private void SaveCurrentSortingAndFiltering(ref string readerName, ref string readerLastName, string bookTitle, string sortOrder,
            string currentFirstNameFilter, string currentLastNameFilter, string currentTitleFilter, ref int? pageNumber)
        {
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.ReaderFirstNameSortParam = sortOrder == "ReaderName" ? "readerName_desc" : "ReaderName";
            ViewBag.ReaderLastNameSortParam = sortOrder == "ReaderLastName" ? "readerLastName_desc" : "ReaderLastName";
            ViewBag.LentToSortParam = sortOrder == "LentTo" ? "lentTo_desc" : "LentTo";
            ViewBag.CurrentSort = sortOrder;

            ViewBag.CurrentFirstNameFilter = SaveFilterValue(ref readerName, currentFirstNameFilter, ref pageNumber);
            ViewBag.CurrentLastNameFilter = SaveFilterValue(ref readerLastName, currentLastNameFilter, ref pageNumber);
            ViewBag.CurrentTitleFilter = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
        }

        private IEnumerable<Loan> SortLoans(IEnumerable<Loan> loans, string sortOrder)
        {
            return sortOrder switch
            {
                "title_desc" => loans.OrderByDescending(l => l.Book.Title),
                "ReaderName" => loans.OrderBy(l => l.Reader.FirstName),
                "readerName_desc" => loans.OrderByDescending(l => l.Reader.FirstName),
                "ReaderLastName" => loans.OrderBy(l => l.Reader.LastName),
                "readerLastName_desc" => loans.OrderByDescending(l => l.Reader.LastName),
                "LentTo" => loans.OrderBy(l => l.LentTo),
                "lentTo_desc" => loans.OrderByDescending(l => l.LentTo),
                _ => loans.OrderBy(l => l.Book.Title)
            };
        }

        // GET: Loans/Details/5
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan loan = await _unitOfWork.LoanRepository
                .GetByIdAsync(id, new[] { nameof(Loan.Book), nameof(Loan.Reader) });

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Return/5
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan loan = await _unitOfWork.LoanRepository
                .GetByIdAsync(id, new[] { nameof(Loan.Book), nameof(Loan.Reader) });

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Books/Return/5
        [HttpPost, ActionName(nameof(Return))]
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> ReturnPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan loan = await _unitOfWork.LoanRepository
                .GetByIdAsync(id, new[] { nameof(Loan.Book), nameof(Loan.Reader) });

            loan.Book.IsBorrowed = false;
            try
            {
                _unitOfWork.LoanRepository.Delete(loan);
                loan.Reader.NumberOfLoans--;
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex);
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
                return View(loan);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
