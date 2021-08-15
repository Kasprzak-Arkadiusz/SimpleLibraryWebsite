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
    public class ReadersController : CustomController
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public ReadersController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<User> userManager)
            : base(context, authorizationService, userManager)
        { }

        // GET: Readers
        public async Task<IActionResult> Index(string readerName, string readerLastName, string sortOrder,
                                                string currentNameFilter, string currentLastNameFilter, int? pageNumber)
        {
            ViewData["ReaderNameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "readerName_desc" : "";
            ViewData["ReaderLastNameSortParam"] = sortOrder == "ReaderLastName" ? "readerLastName_desc" : "ReaderLastName";
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CurrentNameFilter"] = SaveFilterValue(ref readerName, currentNameFilter, ref pageNumber);
            ViewData["CurrentLastNameFilter"] = SaveFilterValue(ref readerLastName, currentLastNameFilter, ref pageNumber);

            var readers = from r in Context.Readers select r;
            if (!string.IsNullOrWhiteSpace(readerName))
            {
                readers = from r in readers where r.FirstName == readerName select r;
            }

            if (!string.IsNullOrWhiteSpace(readerLastName))
            {
                readers = from r in readers where r.LastName == readerLastName select r;
            }

            if (!readers.Any())
            {
                return View(new ReaderViewModel { PaginatedList = new PaginatedList<Reader>() });
            }

            ReaderViewModel readerViewModel = new ReaderViewModel();
            var results = sortOrder switch
            {
                "readerName_desc" => readers.OrderByDescending(r => r.FirstName),
                "ReaderLastName" => readers.OrderBy(r => r.LastName),
                "readerLastName_desc" => readers.OrderByDescending(r => r.LastName),
                _ => readers.OrderBy(r => r.FirstName)
            };

            readerViewModel.Readers = await results.ToListAsync();

            const int pageSize = 1;
            readerViewModel.PaginatedList = PaginatedList<Reader>.Create(readerViewModel.Readers, pageNumber ?? 1, pageSize);
            return View(readerViewModel);
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var reader = await Context.Readers
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
        public async Task<IActionResult> Create([Bind("Name,LastName")] Reader reader)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Context.Add(reader);
                    await Context.SaveChangesAsync();
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

            var reader = await Context.Readers.FindAsync(id);
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
        public async Task<IActionResult> EditPost(string id)
        {
            var readerToUpdate = await Context.Readers.FirstOrDefaultAsync(r => r.ReaderId == id);

            if (await TryUpdateModelAsync(
                readerToUpdate,
                "",
                r => r.FirstName, r => r.LastName))
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

            return View(readerToUpdate);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var reader = await Context.Readers
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
            var reader = await Context.Readers.FindAsync(id);
            if (reader is null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Context.Readers.Remove(reader);
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
