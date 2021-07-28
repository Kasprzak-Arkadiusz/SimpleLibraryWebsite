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
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index(string readerName, string readerSurname, string bookTitle, string sortOrder)
        {
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "author_desc" : "Author";

            var requests = from req in _context.Requests select req;
            var readers = from read in _context.Readers select read;
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
                return View(new RequestViewModel() {Requests = new List<Request>()});
            }

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                requests = from r in requests where r.Title.Contains(bookTitle) select r;
                isAnySearchFieldFilled = true;
            }

            RequestViewModel requestViewModel = new RequestViewModel();

            if (isAnySearchFieldFilled)
            {
                requestViewModel.Requests = requests
                    .Where(r => requests.Any(read => read.ReaderID == r.ReaderID)).ToList();
            }
            else
            {
                requestViewModel.Requests = await requests.ToListAsync();
            }

            var results= sortOrder switch
            {
                "title_desc" => requestViewModel.Requests.OrderByDescending(r => r.Title),
                "Author" => requestViewModel.Requests.OrderBy(r => r.Author),
                "author_desc" => requestViewModel.Requests.OrderByDescending(r => r.Author),
                _ => requestViewModel.Requests.OrderBy(r => r.Title)
            };

            requestViewModel.Requests = results.ToList();

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
