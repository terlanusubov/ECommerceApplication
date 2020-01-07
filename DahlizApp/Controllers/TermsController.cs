using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Data;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using DahlizApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Controllers
{
    public class TermsController : Controller
    {
        private readonly DahlizDb db;
        public TermsController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            int langId = HttpContext.GetLanguage("langId");

            List<TermsLanguage> termsLanguages = await db.TermsLanguages
                                                            .Where(t => t.LanguageId == langId)
                                                                 .Include(t=>t.Terms)    
                                                                    .ToListAsync();


            return View(termsLanguages);
        }
    }
}