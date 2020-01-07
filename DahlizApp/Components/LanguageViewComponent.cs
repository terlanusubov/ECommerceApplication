using DahlizApp.Data;
using DahlizApp.Models;
using DahlizApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using DahlizApp.Core.Extensions;
using System.Threading.Tasks;

namespace DahlizApp.Components
{
    public class LanguageViewComponent : ViewComponent
    {
        private readonly DahlizDb db;

        public LanguageViewComponent(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int langId = HttpContext.GetLanguage("langId");

            LanguageModel model = new LanguageModel()
            {
                Languages = await db.Languages.ToListAsync(),
                Language = await db.Languages.Where(l => langId!=0?(l.Id == langId):true).FirstOrDefaultAsync()
            };
            return View(model);
        }
    }
}
