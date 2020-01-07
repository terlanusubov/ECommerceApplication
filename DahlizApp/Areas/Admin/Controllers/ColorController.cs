using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ColorController : Controller
    {
        private readonly DahlizDb db;
        public ColorController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Color");
            int langId = HttpContext.GetLanguage("adminLangId");
            List<ColorLanguage> colorLanguages = await db.ColorLanguage.Where(cl => cl.LanguageId == langId)
                                                                            .Include(cl=>cl.Color)
                                                                            .ToListAsync();
            var color = colorLanguages.FirstOrDefault();
            return View(colorLanguages);
        }
        public async Task<IActionResult> Add()
        {
            List<Language> languages = await db.Languages.ToListAsync();
            return View(languages);
        }
        [HttpPost]
        public async Task<IActionResult> Add(List<string> Names)
        {
            List<Language> languages = await db.Languages.ToListAsync();
            if (Checker.CheckList(Names))
            {
                Color color = new Color();
                db.Colors.Add(color);
                for (int i = 0; i < languages.Count; i++)
                {
                    ColorLanguage colorLanguage = new ColorLanguage();
                    colorLanguage.Name = Names[i];
                    colorLanguage.LanguageId = languages[i].Id;
                    colorLanguage.ColorId = color.Id;
                    db.ColorLanguage.Add(colorLanguage);
                }
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Fill all Blanks");
                return View();
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            List<ColorLanguage> colorLanguages = await db.ColorLanguage.Where(cl => cl.ColorId == id)
                                                                            .Include(cl => cl.Color)
                                                                            .Include(cl=>cl.Language)
                                                                            .ToListAsync();
            return View(colorLanguages);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(List<string> Names, int? id)
        {
            if(id == null)
            {
                return RedirectToAction("error", "home");
            }
            List<ColorLanguage> colorLanguages = await db.ColorLanguage.Where(cl => cl.ColorId == id).Include(cl=>cl.Language).ToListAsync();
            if (colorLanguages == null)
            {
                return RedirectToAction("error", "home");
            }
            if (Checker.CheckList(Names))
            {
                for (int i = 0; i < colorLanguages.Count; i++)
                {
                    colorLanguages[i].Name = Names[i];
                    db.SaveChanges();
                }
            }
            else
            {
                ModelState.AddModelError("", "Fill all Blanks");
                return View(colorLanguages);
            }
            return RedirectToAction("index", "color");

        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            List<ColorLanguage> colorLanguages = db.ColorLanguage.Where(cl => cl.ColorId == id).ToList();
            Color color = db.Colors.FirstOrDefault(c => c.Id == id);
            if (colorLanguages == null || color==null)
            {
                return RedirectToAction("error", "home");
            }
            db.Colors.Remove(color);
            db.ColorLanguage.RemoveRange(colorLanguages);
            db.SaveChanges();
            return RedirectToAction("index", "color");
        }
    }
}