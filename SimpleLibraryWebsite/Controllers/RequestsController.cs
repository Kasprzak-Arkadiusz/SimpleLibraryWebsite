using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

            const int pageSize = 5;

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

        private static IEnumerable<Request> SortRequests(IEnumerable<Request> requests, string sortOrder)
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
                .GetByIdAsync(id, new[] { nameof(Loan.Reader) });

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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request == null || userId == null)
            {
                return NotFound();
            }

            Reader requestingReader = await _unitOfWork.ReaderRepository.GetByIdAsync(userId);

            if (requestingReader.NumberOfRequests >= Reader.BookRequestLimit)
            {
                ModelState.AddModelError("", "You have exceeded the limit of requested books.\n "
                                             + $"You can request a maximum of {Reader.BookRequestLimit} books.\n"
                                             + "Wait for the librarian to fulfill your request or\n"
                                             + "remove other requests");
                return View(request);
            }

            if (request.AnyFieldIsNullOrEmpty())
            {
                ModelState.AddModelError("", "All fields must be filled");
                return View(request);
            }

            try
            {
                request.Author = request.Author.Trim();
                request.Title = request.Title.Trim();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id is null)
            {
                return NotFound();
            }

            Request requestToUpdate = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (requestToUpdate == null)
            {
                Request deletedRequest = new();
                await TryUpdateModelAsync(deletedRequest);
                ModelState.AddModelError(string.Empty, "Unable to save changes. The request was deleted" 
                                                       + " or fulfilled by another user.");
                return View(deletedRequest);
            }

            _unitOfWork.RequestRepository.SetRowVersionOriginalValue(requestToUpdate, rowVersion);

            if (!await TryUpdateModelAsync(
                requestToUpdate,
                "",
                r => r.Title, r => r.Author, r => r.Genre, r => r.ReaderId)) return View(requestToUpdate);
            try
            {
                requestToUpdate.Author = requestToUpdate.Author.Trim();
                requestToUpdate.Title = requestToUpdate.Title.Trim();

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var exceptionEntry = ex.Entries.Single();
                var clientValues = (Request)exceptionEntry.Entity;
                var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync();
                if (databaseEntry == null)
                {
                    ModelState.AddModelError(string.Empty,
                        "Unable to save changes. The request was deleted by another user.");
                }
                else
                {
                    var databaseValues = (Request)databaseEntry.ToObject();

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
                                                           + "the Save button again. Otherwise click the Back to ist hyperlink.");
                    requestToUpdate.RowVersion = databaseValues.RowVersion;
                    ModelState.Remove("RowVersion");
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
        public async Task<IActionResult> FulfillPost(int? id, byte[] rowVersion)
        {
            if (id is null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository.GetByIdAsync(id);

            if (request == null)
            {
                return RedirectToAction(nameof(Index));
            }

            Reader requestingReader = await _unitOfWork.ReaderRepository.GetByIdAsync(request.ReaderId);
            Book requestedBook = new()
            {
                Author = request.Author,
                Title = request.Title,
                Genre = request.Genre
            };

            _unitOfWork.RequestRepository.SetRowVersionOriginalValue(request, rowVersion);

            try
            {
                requestingReader.NumberOfRequests--;
                _unitOfWork.RequestRepository.Delete(request);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                ViewBag.ConcurrencyError = "The request you attempted to fulfill was modified by another user"
                                           + " after you got the original value. The fulfill operation was"
                                           + " canceled and the current values in the database have been"
                                           + " displayed. If you still want to edit this record, click "
                                           + "the Fulfill button again. Otherwise click the Back to List hyperlink.";
                
                var databaseEntry = await ex.Entries.Single().GetDatabaseValuesAsync();
                var databaseValues = (Request)databaseEntry.ToObject();
                request.RowVersion = databaseValues.RowVersion;
                 ModelState.Remove("RowVersion");

                return View(request);
            }

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
                .GetByIdAsync(id, new[] { nameof(Loan.Reader) })
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
