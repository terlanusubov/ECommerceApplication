using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Areas.Admin.Models;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class BrandController : Controller
    {
        private readonly DahlizDb db;
        public BrandController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Brand");
            int langId = HttpContext.GetLanguage("adminLangId");

            #region Create BrandIndexModel for view
            BrandIndexModel model = new BrandIndexModel();

            model.Brands = await db.Brands.ToListAsync();

            model.CategoryBrands = await db.CategoryBrands
                                                .Include(cb => cb.Category)
                                                        .ToListAsync();

            model.CategoryLanguages = await db.CategoryLanguages
                                                    .Where(cl => cl.LanguageId == langId)
                                                            .ToListAsync();

            #endregion
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Brand");
            int langId = HttpContext.GetLanguage("adminLangId");

            List<CategoryLanguage> categoryLanguages = await db.CategoryLanguages
                                                                        .Where(cl => cl.LanguageId == langId)
                                                                                .ToListAsync();

            return View(categoryLanguages);
        }



        [HttpPost]
        public async Task<IActionResult> Add(BrandModel model)
        {
            int langId = HttpContext.GetLanguage("adminLangId");

            if (ModelState.IsValid && Checker.CheckList(model.Categories))
            {
                #region Create Brand and Some CategoryBrands
                Brand brand = new Brand();
                brand.Name = model.BrandName;
                db.Brands.Add(brand);
                for (var i = 0; i < model.Categories.Count; i++)
                {
                    CategoryBrand categoryBrand = new CategoryBrand();
                    categoryBrand.BrandId = brand.Id;
                    categoryBrand.CategoryId = model.Categories[i];

                    db.CategoryBrands.Add(categoryBrand);
                }

                await db.SaveChangesAsync();
                #endregion

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Make sure that you fill al required blanks");
                return RedirectToAction(nameof(Add));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            int langId = HttpContext.GetLanguage("adminLangId");
            if (id != null)
            {
                #region Creat Brand Model for View
                Brand brand = await db.Brands
                                           .Where(b => b.Id == id)
                                                   .FirstOrDefaultAsync();

                List<CategoryBrand> categoryBrands = await db.CategoryBrands
                                                                    .Where(cb => cb.BrandId == id)
                                                                            .Include(cb => cb.Category)
                                                                                    .ToListAsync();

                List<CategoryLanguage> categoryLanguages = await db.CategoryLanguages
                                                                            .Where(cl=>cl.LanguageId == langId)
                                                                                        .Include(cl=>cl.Category)
                                                                                                .ToListAsync();
                BrandModel brandModel = new BrandModel()
                {
                    Brand = brand,
                    CategoryBrands = categoryBrands,
                    CategoryLanguages = categoryLanguages
                };

                #endregion

                return View(brandModel);
            }
            else
            {
                ModelState.AddModelError("", "Can not find Brand");
                return RedirectToAction(nameof(Index));
            }
            
        }

        public async Task<IActionResult> Edit(BrandModel model)
        {
            if(ModelState.IsValid && Checker.CheckList(model.Categories))
            {
                #region Brand and CategoryBrand edit
                Brand brand = await db.Brands.Where(b => b.Id == model.BrandId).FirstOrDefaultAsync();
                brand.Name = model.BrandName;

                List<CategoryBrand> categoryBrands = await db.CategoryBrands.Where(cb => cb.BrandId == model.BrandId).ToListAsync();

                if (model.Categories.Count > categoryBrands.Count)
                {
                    for (int i = 0; i < model.Categories.Count; i++)
                    {
                        if (i + 1 > categoryBrands.Count)
                        {
                            CategoryBrand categoryBrand = new CategoryBrand();
                            categoryBrand.BrandId = brand.Id;
                            categoryBrand.CategoryId = model.Categories[i];

                            db.CategoryBrands.Add(categoryBrand);
                        }
                        else
                        {
                            categoryBrands[i].CategoryId = model.Categories[i];
                        }
                    }
                }
                else if (model.Categories.Count == categoryBrands.Count)
                {
                    for (int i = 0; i < model.Categories.Count; i++)
                    {
                        categoryBrands[i].CategoryId = model.Categories[i];
                    }
                }
                else
                {
                    for (int i = 0; i < categoryBrands.Count; i++)
                    {
                        if (i + 1 <= model.Categories.Count)
                        {
                            categoryBrands[i].CategoryId = model.Categories[i];
                        }
                        else
                        {
                            CategoryBrand categoryBrand = db.CategoryBrands
                                                            .Where(cb => cb.CategoryId == categoryBrands[i].CategoryId)
                                                                    .FirstOrDefault();
                            db.CategoryBrands.Remove(categoryBrand);
                        }
                    }

                }
                #endregion
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Fill all required blanks");
                return RedirectToAction(nameof(Edit));
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? brandId)
        {
            if(brandId != null)
            {
                #region Find Brands And All Category Brands and delete them
                Brand brand = await db.Brands
                                        .Where(b => b.Id == brandId)
                                             .FirstOrDefaultAsync();

                List<CategoryBrand> categoryBrands = await db.CategoryBrands
                                                            .Where(cb => cb.BrandId == brandId)
                                                                    .ToListAsync();


                db.CategoryBrands.RemoveRange(categoryBrands);
                db.Brands.Remove(brand);

                await db.SaveChangesAsync();
                #endregion

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Can not find Brand");
                return RedirectToAction(nameof(Index));
            }
            

        }
    }
}