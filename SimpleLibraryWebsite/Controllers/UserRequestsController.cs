using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Data.DAL;
using SimpleLibraryWebsite.Models;
using SimpleLibraryWebsite.Services.Authorization;
using X.PagedList;

namespace SimpleLibraryWebsite.Controllers
{
    [AuthorizeWithEnumRoles(Role.Reader, Role.Admin)]
    public class UserRequestsController : Controller
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly UnitOfWork _unitOfWork;

        public UserRequestsController(ApplicationDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = _unitOfWork.RequestRepository.Get(r => r.ReaderId == userId);

            return View(await requests.ToListAsync());
        }


        // GET: Requests/Edit/5
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

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Request request = await _unitOfWork.RequestRepository.GetByIdAsync(id, new[] { nameof(Loan.Reader) });

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
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
