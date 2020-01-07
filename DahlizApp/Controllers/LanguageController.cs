using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using DahlizApp.Data;

namespace DahlizApp.Controllers
{
    public class LanguageController : Controller
    {
        private readonly DahlizDb db;
        public LanguageController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            await HttpContext.SetLanguage(culture, db,"langId");
            return LocalRedirect(returnUrl);
        }
    }
}