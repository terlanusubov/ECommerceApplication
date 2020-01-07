using DahlizApp.Core.Extensions;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Components
{
    public class NavbarViewComponent:ViewComponent
    {
        private readonly DahlizDb db;

        public NavbarViewComponent(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int langId = HttpContext.GetLanguage("langId");
            if(langId == 0)
            {
                Language lang = await db.Languages.FirstOrDefaultAsync();
                langId = lang.Id;
            }
            HomeIndexModel model = new HomeIndexModel();
            model.Ads = await db.Ads.ToListAsync();
            model.CategoryLanguages = await db.CategoryLanguages.Where(cl => cl.LanguageId == langId).Include(cl=>cl.Category).ThenInclude(c=>c.Subcategories).ToListAsync();
            model.SubcategoryLanguages = await db.SubcategoryLanguages.Where(sl => sl.LanguageId == langId).Include(sl=>sl.Subcategory).ToListAsync();
            model.Slides = await db.Slides.ToListAsync();
            model.Language = await db.Languages.Where(l => l.Id == langId).FirstOrDefaultAsync();
            model.Languages = await db.Languages.ToListAsync();
            model.CategoryBrands = await db.CategoryBrands.Include(cb=>cb.Brand).ToListAsync();
            model.NotiLanguage = await db.NotiLanguage.Include(nl=>nl.Noti).FirstOrDefaultAsync(nl=>nl.LanguageId==langId);
            if (HttpContext.Session.GetObjectFromJson<Card>("Card") != null)
            {
                model.Cards = HttpContext.Session.GetObjectFromJson<Card>("Card");
            }
            else
            {
                model.Cards = new List<Card>();
            }
            return View(model);
        }
    }
}
