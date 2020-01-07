using DahlizApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Components
{
    public class FooterViewComponent:ViewComponent
    {
        private readonly DahlizDb db;
        public FooterViewComponent(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int langId = HttpContext.GetLanguage("langId");

            FooterModel footerModel = new FooterModel()
            {
                Setting = await db.Settings.FirstOrDefaultAsync(),
                SubcategoryLanguages = await db.SubcategoryLanguages.Where(scl => scl.LanguageId == langId).ToListAsync()
            };
            return View(footerModel);
        }
    }
}
