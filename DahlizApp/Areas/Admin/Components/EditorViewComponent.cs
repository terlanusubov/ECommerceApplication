using DahlizApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Components
{
    public class EditorViewComponent:ViewComponent
    {
        private readonly DahlizDb db;
        public EditorViewComponent(DahlizDb _db)
        {
            db = _db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var languages = await db.Languages.ToListAsync();
            return View(languages);
        }
    }
}
