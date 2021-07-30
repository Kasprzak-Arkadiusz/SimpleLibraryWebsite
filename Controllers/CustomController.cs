using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SimpleLibraryWebsite.Controllers
{
    public class CustomController : Controller
    {
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
