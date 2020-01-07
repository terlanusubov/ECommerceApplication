using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Controllers
{
    public class AboutController : Controller
    {
        private readonly DahlizDb db;
        public AboutController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            int langId = HttpContext.GetLanguage("langId");

            List<AboutLanguage> aboutLanguages = await db.AboutLanguages
                                                            .Where(t => t.LanguageId == langId)
                                                                 .Include(t => t.About)
                                                                    .ToListAsync();


            return View(aboutLanguages);
        }
    }
}