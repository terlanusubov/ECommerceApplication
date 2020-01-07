using DahlizApp.Core.Extensions;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Components
{
    public class AdminLanguageViewComponent : ViewComponent
    {
        private readonly DahlizDb db;

        public AdminLanguageViewComponent(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            List<Language> languages = await db.Languages.ToListAsync();
            return View(languages);

        }
    }
}
