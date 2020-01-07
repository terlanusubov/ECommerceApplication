using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Core.Extensions;
using DahlizApp.Data;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Models;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SubCategoryController : Controller
    {
        private readonly DahlizDb db;
        public SubCategoryController(DahlizDb _db)
        {
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            SubcategoryViewModel model = new SubcategoryViewModel();
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("index", "subcategory");
            int langId = HttpContext.GetLanguage("adminLangId");

            model.subcategoryLanguages = await db.SubcategoryLanguages.Where(sl => sl.LanguageId == langId)
                                                                                            .Include(sl=>sl.Subcategory)
                                                                                            .ToListAsync();

            model.categoryLanguages = await db.CategoryLanguages.Where(cl => cl.LanguageId == langId)
                                                                                            .Include(cl => cl.Category)
                                                                                            .ToListAsync();
            return View(model);
        }
        public async Task<IActionResult> Add()
        {
            int langId = HttpContext.GetLanguage("adminLangId");
            List<Language> languages = await db.Languages.ToListAsync();
            List<CategoryLanguage> categoryLanguages = await db.CategoryLanguages.Where(cl=>cl.LanguageId==langId)
                                                                                                    .Include(cl=>cl.Category)
                                                                                                    .Include(cl=>cl.Language)
                                                                                                    .ToListAsync();

            SubcategoryViewModel model = new SubcategoryViewModel()
            {
                Languages = languages,
                categoryLanguages = categoryLanguages
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(List<string> Names,int CategoryId)
        {
            if (Checker.CheckList(Names))
            {
                Subcategory subcategory = new Subcategory();
                db.Subcategories.Add(subcategory);
                List<Language> languages = db.Languages.ToList();
                for(int i = 0; i < languages.Count; i++)
                {
                    SubcategoryLanguage subcategoryLanguage = new SubcategoryLanguage();
                    subcategoryLanguage.Subcategory = subcategory;
                    subcategoryLanguage.SubcategoryId = subcategory.Id;
                    subcategoryLanguage.Subcategory.CategoryId = CategoryId;
                    subcategoryLanguage.Name = Names[i];
                    subcategoryLanguage.LanguageId = languages[i].Id;
                    db.SubcategoryLanguages.Add(subcategoryLanguage);
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


        public async Task<IActionResult> Edit(int id)
        {
            int langId = HttpContext.GetLanguage("adminLangId");
            List<CategoryLanguage> categoryLanguages = await db.CategoryLanguages.Where(cl => cl.LanguageId == langId)
                                                                                                .Include(cl => cl.Category)
                                                                                                .Include(cl => cl.Language)
                                                                                                .ToListAsync();

            List<SubcategoryLanguage> subcategoryLanguages = await db.SubcategoryLanguages.Where(sl => sl.SubcategoryId == id)
                                                                                                .Include(sl => sl.Subcategory)
                                                                                                .Include(sl=>sl.Language)
                                                                                                .ToListAsync();

            SubcategoryViewModel model = new SubcategoryViewModel();
            model.subcategoryLanguages = subcategoryLanguages;
            model.categoryLanguages = categoryLanguages;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(List<string> Names,int CategoryId,int SubcategoryId)
        {
            List<SubcategoryLanguage> subcategoryLanguages = await db.SubcategoryLanguages.Where(sl => sl.SubcategoryId == SubcategoryId)
                                                                                                    .Include(sl => sl.Subcategory)
                                                                                                    .Include(sl => sl.Language)
                                                                                                    .ToListAsync();

            CategoryLanguage categoryLanguage = await db.CategoryLanguages.Where(cl=>cl.CategoryId == CategoryId)
                                                                                                 .Include(cl=>cl.Category)
                                                                                                 .Include(cl=>cl.Language)
                                                                                                 .FirstOrDefaultAsync();


            if (Checker.CheckList(Names))
            {
                for (int i = 0; i < subcategoryLanguages.Count; i++)
                {
                    subcategoryLanguages[i].Name = Names[i];
                    subcategoryLanguages[i].Subcategory.Category = categoryLanguage.Category;
                    subcategoryLanguages[i].Subcategory.CategoryId = categoryLanguage.Category.Id;

                    await db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {

                ModelState.AddModelError("", "Fill all Blanks");
                return RedirectToAction("Edit", "Subcategory", new { id = SubcategoryId });
            }
            
        }


        public IActionResult Delete(int? subcategoryId)
        {
            if (subcategoryId == null)
            {
                return RedirectToAction("error", "home");
            }
            List<SubcategoryLanguage> subcategoryLanguages = db.SubcategoryLanguages.Where(cl => cl.SubcategoryId == subcategoryId).ToList();
            Subcategory subcategory = db.Subcategories.Find(subcategoryId);
            db.Subcategories.Remove(subcategory);
            db.SubcategoryLanguages.RemoveRange(subcategoryLanguages);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}