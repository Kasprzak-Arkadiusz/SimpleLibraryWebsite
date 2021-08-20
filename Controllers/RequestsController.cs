using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Data.DAL;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;
using X.PagedList;

namespace SimpleLibraryWebsite.Controllers
{
    public class RequestsController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly UnitOfWork _unitOfWork;

        public RequestsController(ApplicationDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: Requests
        public async Task<IActionResult> Index(string bookTitle, string author, string sortOrder,
                                                string currentTitleFilter, string currentAuthorFilter, int? pageNumber)
        {
            SaveCurrentSortingAndFiltering(ref bookTitle, ref author, sortOrder,
                 currentTitleFilter, currentAuthorFilter, ref pageNumber);

            var requests = _unitOfWork.RequestRepository.Get();

            if (!string.IsNullOrWhiteSpace(author))
            {
                requests = requests.Where(r => r.Author.Contains(author));
            }

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                requests = requests.Where(r => r.Title.Contains(bookTitle));
            }

            requests = SortRequests(requests, sortOrder);

            const int pageSize = 1;

            RequestViewModel requestViewModel = new()
            {
                PaginatedList = await requests.ToPagedListAsync(pageNumber ?? 1, pageSize)
            };

            return View(requestViewModel);
        }

        private void SaveCurrentSortingAndFiltering(ref string bookTitle, ref string author, string sortOrder,
            string currentTitleFilter, string currentAuthorFilter, ref int? pageNumber)
        {
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.AuthorSortParam = sortOrder == "Author" ? "author_desc" : "Author";
            ViewBag.CurrentSort = sortOrder;

            ViewBag.CurrentTitleFilter = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewBag.CurrentAuthorFilter = SaveFilterValue(ref author, currentAuthorFilter, ref pageNumber);
        }

        private IQueryable<Request> SortRequests(IQueryable<Request> requests, string sortOrder)
        {
            return sortOrder switch
            {
                "title_desc" => requests.OrderByDescending(b => b.Title),
                "Author" => requests.OrderBy(b => b.Author),
                "author_desc" => requests.OrderByDescending(b => b.Author),
                _ => requests.OrderBy(b => b.Title)
            };
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository
                .GetByIdAsync(id.Value, new[] { nameof(Loan.Reader) });

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Requests/Create
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Requests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Create([Bind("Title,Author,Genre")] Request request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Reader requestingReader = await _unitOfWork.ReaderRepository.GetByIdAsync(userId);

            if (requestingReader.NumberOfRequests >= Reader.BookRequestLimit)
            {
                ModelState.AddModelError("", "You have exceeded the limit of requested books.\n "
                                             + $"You can request a maximum of {Reader.BookRequestLimit} books.\n"
                                             + "Wait for the librarian to fulfill your request or\n"
                                             + "remove other requests");
                return View(request);
            }

            if (request == null || userId == null)
            {
                return NotFound();
            }

            if (request.AnyFieldIsNullOrEmpty())
            {
                ModelState.AddModelError("", "All fields must be filled");
                return View(request);
            }

            try
            {
                request.ReaderId = userId;
                requestingReader.NumberOfRequests++;
                await _unitOfWork.RequestRepository.InsertAsync(request);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(request);
        }

        // GET: Requests/Edit/5
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Edit/5
        [HttpPost, ActionName(nameof(Edit))]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Request requestToUpdate = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (await TryUpdateModelAsync(
                requestToUpdate,
                "",
                r => r.Title, r => r.Author, r => r.Genre, r => r.ReaderId))
            {
                try
                {
                    await _unitOfWork.SaveAsync();
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

            return View(requestToUpdate);
        }

        // GET: Requests/Fulfill/5
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> Fulfill(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        
        // POST: Requests/Fulfill/5
        [HttpPost, ActionName(nameof(Fulfill))]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Librarian, Role.Admin)]
        public async Task<IActionResult> FulfillPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            Book requestedBook = new()
            {
                Author = request.Author,
                Title = request.Title,
                Genre = request.Genre
            };

            _unitOfWork.RequestRepository.Delete(request);
            await _unitOfWork.SaveAsync();

            return RedirectToActionPreserveMethod("Create", "Books", requestedBook);
        }

        // GET: Requests/Delete/5
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository
                .GetByIdAsync(id.Value, new[] { nameof(Loan.Reader) })
                ;
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Request request = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (request is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _unitOfWork.RequestRepository.Delete(request);
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
