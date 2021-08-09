using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class RequestsController : CustomController
    {
        private readonly ApplicationDbContext _context;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index(string bookTitle, string author, string sortOrder,
                                                string currentTitleFilter, string currentAuthorFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentTitleFilter"] = SaveFilterValue(ref bookTitle, currentTitleFilter, ref pageNumber);
            ViewData["CurrentAuthorFilter"] = SaveFilterValue(ref author, currentAuthorFilter, ref pageNumber);

            var requests = from req in _context.Requests select req;
            if (!string.IsNullOrWhiteSpace(author))
            {
                requests = from r in requests where r.Author == author select r;
            }

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                requests = from r in requests where r.Title.Contains(bookTitle) select r;
            }

            if (!requests.Any())
            {
                return View(new RequestViewModel() { PaginatedList = new PaginatedList<Request>() });
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
            requestViewModel.PaginatedList = PaginatedList<Request>.Create(requestViewModel.Requests, pageNumber ?? 1, pageSize);

            return View(requestViewModel);
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
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
            var readerIdList = (from r in _context.Readers
                                select new SelectListItem()
                                {
                                    Text = r.ReaderId.ToString(),
                                    Value = r.ReaderId.ToString()
                                }).ToList();

            ViewBag.ListOfReaderId = readerIdList;
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            CreateReaderIdList();
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReaderId,Title,Author,Genre")] Request request)
        {
            try
            {
                CreateReaderIdList();
                request.FillMissingProperties(await _context.Readers.FindAsync(request.ReaderId));
                _context.Add(request);
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

            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            CreateReaderIdList();

            return View(request);
        }

        // POST: Requests/Edit/5
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

            var requestToUpdate = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == id);

            if (await TryUpdateModelAsync(
                requestToUpdate,
                "",
                r => r.Title, r => r.Author, r => r.Genre, r => r.ReaderId, r => r.NumberOfUpvotes))
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

            return View(requestToUpdate);
        }

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Reader)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Requests.Remove(request);
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
