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
    public class NotiController : Controller
    {
        private readonly DahlizDb db;
        public NotiController(DahlizDb _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Noti");
            int langId = HttpContext.GetLanguage("adminLangId");
            NotiLanguage notiLanguage = db.NotiLanguage.Include(nl => nl.Noti).FirstOrDefault(nl => nl.LanguageId == langId);

            return View(notiLanguage);
        }
        public async Task<IActionResult> Add()
        {
            List<Language> languages = await db.Languages.ToListAsync();
            return View(languages);
        }
        [HttpPost]
        public async Task<IActionResult> Add(List<string> Texts,string Link)
        {
            List<Language> languages = await db.Languages.ToListAsync();
            if (Checker.CheckList(Texts))
            {
                Noti noti = new Noti();
                noti.Link = Link;
                db.Noti.Add(noti);
                for (int i = 0; i < languages.Count; i++)
                {
                    NotiLanguage notiLanguage = new NotiLanguage();
                    notiLanguage.Text = Texts[i];
                    notiLanguage.LanguageId = languages[i].Id;
                    notiLanguage.NotiId = noti.Id;
                    
                    db.NotiLanguage.Add(notiLanguage);
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
            List<NotiLanguage> notiLanguages = await db.NotiLanguage.Where(nl => nl.NotiId == id)
                                                                            .Include(nl => nl.Noti)
                                                                            .Include(nl => nl.Language)
                                                                            .ToListAsync();
            return View(notiLanguages);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(List<string> Texts,string Link, int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            List<NotiLanguage> notiLanguage = await db.NotiLanguage.Where(nl => nl.NotiId == id).Include(nl => nl.Language).ToListAsync();
            if (notiLanguage == null)
            {
                return RedirectToAction("error", "home");
            }
            if (Checker.CheckList(Texts))
            {
                for (int i = 0; i < notiLanguage.Count; i++)
                {
                    notiLanguage[i].Text = Texts[i];
                    db.SaveChanges();
                }
            }
            else
            {
                ModelState.AddModelError("", "Fill all Blanks");
                return View(notiLanguage);
            }
            return RedirectToAction("index", "noti");

        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            List<NotiLanguage> notiLanguage = db.NotiLanguage.Where(nl => nl.NotiId == id).ToList();
            Noti noti = db.Noti.FirstOrDefault(n => n.Id == id);
            if (notiLanguage == null || noti == null)
            {
                return RedirectToAction("error", "home");
            }
            db.Noti.Remove(noti);
            db.NotiLanguage.RemoveRange(notiLanguage);
            db.SaveChanges();
            return RedirectToAction("index", "noti");
        }
    }
}