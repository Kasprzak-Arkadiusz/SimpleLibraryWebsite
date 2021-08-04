using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class ReadersController : CustomController
    {
        private readonly ApplicationDbContext _context;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public ReadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index(string readerName, string readerSurname, string sortOrder,
                                                string currentNameFilter, string currentSurnameFilter, int? pageNumber)
        {
            ViewData["ReaderNameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "readerName_desc" : "";
            ViewData["ReaderSurnameSortParam"] = sortOrder == "ReaderSurname" ? "readerSurname_desc" : "ReaderSurname";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentNameFilter"] = SaveFilterValue(ref readerName, currentNameFilter, ref pageNumber);
            ViewData["CurrentSurnameFilter"] = SaveFilterValue(ref readerSurname, currentSurnameFilter, ref pageNumber);

            var readers = from r in _context.Readers select r;
            if (!string.IsNullOrWhiteSpace(readerName))
            {
                readers = from r in readers where r.Name == readerName select r;
            }

            if (!string.IsNullOrWhiteSpace(readerSurname))
            {
                readers = from r in readers where r.Surname == readerSurname select r;
            }

            if (!readers.Any())
            {
                return View(new ReaderViewModel { PaginatedList = new PaginatedList<Reader>() });
            }

            ReaderViewModel readerViewModel = new ReaderViewModel();
            var results = sortOrder switch
            {
                "readerName_desc" => readers.OrderByDescending(r => r.Name),
                "ReaderSurname" => readers.OrderBy(r => r.Surname),
                "readerSurname_desc" => readers.OrderByDescending(r => r.Surname),
                _ => readers.OrderBy(r => r.Name)
            };

            readerViewModel.Readers = await results.ToListAsync();

            const int pageSize = 1;
            readerViewModel.PaginatedList = PaginatedList<Reader>.Create(readerViewModel.Readers, pageNumber ?? 1, pageSize);
            return View(readerViewModel);
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // GET: Readers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Readers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Surname")] Reader reader)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(reader);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(reader);
        }

        // GET: Readers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }
            return View(reader);
        }

        // POST: Readers/Edit/5
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

            var readerToUpdate = await _context.Readers.FirstOrDefaultAsync(r => r.ReaderId == id);

            if (await TryUpdateModelAsync(
                readerToUpdate,
                "",
                r => r.Name, r => r.Surname))
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

            return View(readerToUpdate);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Readers.Remove(reader);
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
