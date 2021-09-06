using Microsoft.AspNetCore.Mvc;

namespace SimpleLibraryWebsite.Controllers
{
    public class CustomController : Controller
    {
        protected static string SaveFilterValue(ref string value, string valueToSave, ref int? pageNumber)
        {
            if (value is not null)
            {
                pageNumber = 1;
            }
            return value ??= valueToSave;
        }
    }
}
