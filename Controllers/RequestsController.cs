using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

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
        private string SaveFilterValue(ref string value, string valueToSave, ref int? pageNumber)
        {
            if (value is not null)
            {
                pageNumber = 1;
            }

            return value ??= valueToSave;
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
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestID,ReaderID,Title,Author,Genre")] Request request)
        {
            if (ModelState.IsValid)
            {
                request.FillMissingProperties(await _context.Readers.FindAsync(request.ReaderID));
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID", request.ReaderID);
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
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID", request.ReaderID);
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestID,ReaderID,Title,Author,Genre,NumberOfUpvotes")] Request request)
        {
            if (id != request.RequestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.RequestID))
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
            ViewData["ReaderID"] = new SelectList(_context.Readers, "ReaderID", "ReaderID", request.ReaderID);
            return View(request);
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
                .FirstOrDefaultAsync(m => m.RequestID == id);
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
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.RequestID == id);
        }
    }
}
