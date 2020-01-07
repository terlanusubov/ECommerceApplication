using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using DahlizApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Authorize(Roles= "Admin")]
    [Area("Admin")]
    public class TermsController : Controller
    {
        private readonly DahlizDb db;
        public TermsController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Terms = "true";

            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Terms");
            int langId = HttpContext.GetLanguage("adminLangId");

            List<TermsLanguage> terms = await db.TermsLanguages.Where(t => t.LanguageId == langId).Include(t => t.Terms).ToListAsync();
            return View(terms);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Languages = await db.Languages.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Add(List<string> Data)
        {
            if (Checker.CheckList(Data))
            {
                Terms term = new Terms();
                db.Terms.Add(term);
                var langCount = await db.Languages.ToListAsync();
                for (int i = 0; i < langCount.Count; i++)
                {
                   
                    TermsLanguage termsLanguage = new TermsLanguage()
                    {
                        Language = langCount[i],
                        Terms = term,
                        TermsId = term.Id,
                        LanguageId = langCount[i].Id,
                        Data = Data[i]
                    };

                    db.TermsLanguages.Add(termsLanguage);
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
                List<TermsLanguage> termsLanguages = await db.TermsLanguages.Where(t => t.TermsId == Id).ToListAsync();
                if(termsLanguages.Count == 1)
                {
                    Terms term = await db.Terms.Where(t => t.Id == Id).FirstOrDefaultAsync();
                    db.Terms.Remove(term);
                }

                TermsLanguage termLanguage = await db.TermsLanguages.Where(t => t.TermsId == Id && t.LanguageId == langId).FirstOrDefaultAsync();
                db.TermsLanguages.Remove(termLanguage);

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
            List<TermsLanguage> termsLanguages = await db.TermsLanguages.Where(t=>t.TermsId == Id).ToListAsync();
            return View(termsLanguages);
        }


        [HttpPost]
        public async Task<JsonResult> Edit(List<string> Data,int Id)
        {
            if(Checker.CheckList(Data) && Id != 0)
            {
                List<TermsLanguage> termsLanguages = await db.TermsLanguages
                                                                .Where(t => t.TermsId == Id)
                                                                    .ToListAsync();
                for (int i = 0; i < Data.Count; i++)
                {
                    termsLanguages[i].Data = Data[i];
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