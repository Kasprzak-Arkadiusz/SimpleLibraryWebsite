using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleLibraryWebsite.Data;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Controllers
{
    public class CustomController : Controller
    {
        protected ApplicationDbContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<User> UserManager { get; }

        public CustomController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<User> userManager)
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        }

        protected string SaveFilterValue(ref string value, string valueToSave, ref int? pageNumber)
        {
            if (value is not null)
            {
                pageNumber = 1;
            }
            return value ??= valueToSave;
        }
    }
}
