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
    public class CategoryController : Controller
    {
        private readonly DahlizDb db;

        public CategoryController(DahlizDb _db)
        {
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index","Category");
            int langId = HttpContext.GetLanguage("adminLangId");
            List<CategoryLanguage> categories = await db.CategoryLanguages.Where(cl => cl.LanguageId == langId).Include(cl => cl.Category).ToListAsync();
            return View(categories);
        }


        public async Task<IActionResult> Add()
        {
            ViewBag.Active = "Shoes";
            List<Language> languages = await db.Languages.ToListAsync();
            return View(languages);
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<string> Names)
        {
            foreach (var item in Names)
            {
                if (item == null)
                {
                    ModelState.AddModelError("", "Name is required");
                    return View();
                }
            }
            Category category = new Category();
            db.Categories.Add(category);
            List<Language> languages = db.Languages.ToList();
            for (int i = 0; i < languages.Count(); i++)
            {
                CategoryLanguage categoryLanguage = new CategoryLanguage();
                categoryLanguage.Name = Names[i];
                categoryLanguage.CategoryId = category.Id;
                categoryLanguage.LanguageId = languages[i].Id;
                db.CategoryLanguages.Add(categoryLanguage);
            }
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            List<CategoryLanguage> categoryLanguages =  db.CategoryLanguages.Where(cl => cl.CategoryId == id).Include(cl=>cl.Category).Include(cl=>cl.Language).ToList();
            if (categoryLanguages == null)
            {
                return RedirectToAction("error", "home");
            }
            return View(categoryLanguages);
        }
        [HttpPost]
        public IActionResult Edit(List<string> Names,int CategoryId)
        {
            List<CategoryLanguage> categoryLanguages = db.CategoryLanguages.Where(cl => cl.CategoryId == CategoryId).Include(cl=>cl.Language).ToList();
            if (Checker.CheckList(Names))
            {
                for(int i=0; i < categoryLanguages.Count; i++)
                {
                    categoryLanguages[i].Name = Names[i];
                }
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Fill all Blanks");
                return View(categoryLanguages);
            }
            return RedirectToAction("index", "category");
        }
        public IActionResult Delete(int? categoryId)
        {
            if (categoryId == null)
            {
                return RedirectToAction("error", "home");
            }
            List<CategoryLanguage> categoryLanguages = db.CategoryLanguages.Where(cl => cl.CategoryId == categoryId).ToList();
            Category category = db.Categories.Find(categoryId);
            db.Categories.Remove(category);
            db.CategoryLanguages.RemoveRange(categoryLanguages);
            db.SaveChanges();
            return RedirectToAction("index", "category");
        }
    }
}