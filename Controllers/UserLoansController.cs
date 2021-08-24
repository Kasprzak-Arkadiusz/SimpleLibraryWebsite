using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Data.DAL;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
    public class UserLoansController : CustomController
    {
        private readonly UnitOfWork _unitOfWork;

        public UserLoansController(ApplicationDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loans = _unitOfWork.LoanRepository.Get(l => l.ReaderId == userId, includeProperties: nameof(Loan.Book));

            return View(await loans.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan loan = await _unitOfWork.LoanRepository
                .GetByIdAsync(id.Value, new[] { nameof(Loan.Book) });

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }
    }
}
