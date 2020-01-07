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
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AboutController : Controller
    {
        private readonly DahlizDb db;
        public AboutController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Terms = "true";

            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "About");
            int langId = HttpContext.GetLanguage("adminLangId");

            List<AboutLanguage> aboutLanaguages = await db.AboutLanguages.Where(t => t.LanguageId == langId).Include(t => t.About).ToListAsync();
            return View(aboutLanaguages);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Add", "About");
            int langId = HttpContext.GetLanguage("adminLangId");

            ViewBag.Languages = await db.Languages.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Add(List<string> Data)
        {
            if (Checker.CheckList(Data))
            {
                About about = new About();
                await db.Abouts.AddAsync(about);
                var langCount = await db.Languages.ToListAsync();
                for (int i = 0; i < langCount.Count; i++)
                {

                    AboutLanguage aboutLanguage = new AboutLanguage()
                    {
                        Language = langCount[i],
                        About = about,
                        LanguageId = langCount[i].Id,
                        Text = Data[i]
                    };

                    await db.AboutLanguages.AddAsync(aboutLanguage);
                    await db.SaveChangesAsync();
                }

                return Json(new { status = 200 });
            }
            else
            {
                return Json(new { status = 400 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            int langId = HttpContext.GetLanguage("adminLangId");

            if (Id != 0)
            {
                List<AboutLanguage> aboutLanguages = await db.AboutLanguages.Where(t => t.AboutId == Id).ToListAsync();
                if (aboutLanguages.Count == 1)
                {
                    About about = await db.Abouts.Where(t => t.Id == Id).FirstOrDefaultAsync();
                    db.Abouts.Remove(about);
                }

                AboutLanguage aboutLanguage = await db.AboutLanguages.Where(t => t.AboutId == Id && t.LanguageId == langId).FirstOrDefaultAsync();
                db.AboutLanguages.Remove(aboutLanguage);

                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            ViewBag.EditCkEditor = "true";
            ViewBag.Languages = await db.Languages.ToListAsync();
            List<AboutLanguage> aboutLanguages = await db.AboutLanguages.Where(t => t.AboutId == Id).Include(b=>b.About).ToListAsync();
            return View(aboutLanguages);
        }


        [HttpPost]
        public async Task<JsonResult> Edit(List<string> Data, int Id)
        {
            if (Checker.CheckList(Data) && Id != 0)
            {
                List<AboutLanguage> aboutLanguages = await db.AboutLanguages
                                                                .Where(t => t.AboutId == Id)
                                                                    .ToListAsync();
                for (int i = 0; i < Data.Count; i++)
                {
                    aboutLanguages[i].Text = Data[i];
                }

                await db.SaveChangesAsync();

                return Json(new { status = 200 });
            }
            else
            {
                return Json(new { status = 400 });
            }
        }
    }
}