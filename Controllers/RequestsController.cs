using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Models.ViewModels;
using X.PagedList;

namespace SimpleLibraryWebsite.Controllers
{
    public class RequestsController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public RequestsController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<User> userManager)
            : base(context, authorizationService, userManager)
        { }

        // GET: Requests
        public async Task<IActionResult> Index(string bookTitle, string author, string sortOrder,
                                                string currentTitleFilter, string currentAuthorFilter, int? pageNumber)
        {
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.AuthorSortParam = sortOrder == "Author" ? "author_desc" : "Author";
            ViewBag.CurrentSort = sortOrder;

            ViewBag.CurrentTitleFilter = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewBag.CurrentAuthorFilter = SaveFilterValue(ref author, currentAuthorFilter, ref pageNumber);

            var requests = from req in Context.Requests select req;
            if (!string.IsNullOrWhiteSpace(author))
            {
                requests = from r in requests where r.Author == author select r;
            }

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                requests = from r in requests where r.Title.Contains(bookTitle) select r;
            }

            RequestViewModel requestViewModel = new RequestViewModel
            {
                Requests = await requests.ToListAsync()
            };

            var results = sortOrder switch
            {
                "title_desc" => requestViewModel.Requests.OrderByDescending(r => r.Title),
                "Author" => requestViewModel.Requests.OrderBy(r => r.Author),
                "author_desc" => requestViewModel.Requests.OrderByDescending(r => r.Author),
                _ => requestViewModel.Requests.OrderBy(r => r.Title)
            };
            requestViewModel.Requests = results.ToList();

            const int pageSize = 1;
            requestViewModel.PaginatedList = requestViewModel.Requests.ToPagedList(pageNumber ?? 1, pageSize);

            return View(requestViewModel);
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await Context.Requests
                .Include(r => r.Reader).AsNoTracking()
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        private void CreateReaderIdList()
        {
            var readerIdList = (from r in Context.Readers
                                select new SelectListItem()
                                {
                                    Text = r.ReaderId.ToString(),
                                    Value = r.ReaderId.ToString()
                                }).ToList();

            ViewBag.ListOfReaderId = readerIdList;
        }

        // GET: Requests/Create
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Requests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Create([Bind("Title,Author,Genre")] Request request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Reader requestingReader = await Context.Readers.SingleOrDefaultAsync(r => r.ReaderId == userId);

            if (requestingReader.NumberOfRequests == Reader.BookRequestLimit)
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
                Context.Add(request);
                await Context.SaveChangesAsync();
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
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await Context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            CreateReaderIdList();

            return View(request);
        }

        // POST: Requests/Edit/5
        [HttpPost, ActionName(nameof(Edit))]
        [ValidateAntiForgeryToken]
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var requestToUpdate = await Context.Requests.FirstOrDefaultAsync(r => r.RequestId == id);

            if (await TryUpdateModelAsync(
                requestToUpdate,
                "",
                r => r.Title, r => r.Author, r => r.Genre, r => r.ReaderId))
            {
                try
                {
                    await Context.SaveChangesAsync();
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

        // GET: Requests/Delete/5
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await Context.Requests
                .Include(r => r.Reader)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [AuthorizeEnum(Role.Reader, Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await Context.Requests.FindAsync(id);
            if (request is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Context.Requests.Remove(request);
                await Context.SaveChangesAsync();
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
