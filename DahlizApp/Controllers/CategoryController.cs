using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using DahlizApp.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DahlizDb db;
        public CategoryController(DahlizDb _db)
        {
            db = _db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, int? subcategoryId)
        {
            HttpContext.Session.SetString("LoadMore", "0");

            if (categoryId != null || subcategoryId != null)
            {
                HttpContext.Session.SetString("categoryId", categoryId.ToString());
                HttpContext.Session.SetString("subcategoryId", subcategoryId.ToString());
            }


            int langId = HttpContext.GetLanguage("langId");
            if (langId == 0)
            {
                return RedirectToAction("SetLanguage", "Language", new { culture = "en", returnUrl = "/" });
            }

            if (categoryId == null && subcategoryId == null)
            {
                string s = HttpContext.Session.GetString("categoryId");
                int catId = HttpContext.Session.GetString("categoryId") != null ? Convert.ToInt32(HttpContext.Session.GetString("categoryId")) : 0;
                int subId = HttpContext.Session.GetString("subcategoryId") != null ? Convert.ToInt32(HttpContext.Session.GetString("subcategoryId")) : 0;

                return RedirectToAction("index", "category", new { categoryId = catId, subcategoryId = subId });
            }


            CategoryModel model = new CategoryModel();

            model.ProductLanguages = await db.ProductLanguages.Where(pl => pl.LanguageId == langId &&
                                                                       (subcategoryId != null ? pl.Product.SubcategoryId == subcategoryId : true
                                                                                && categoryId != null ? pl.Product.Subcategory.CategoryId == categoryId : true))
                                                                                    .Include(pl => pl.Product).Include(p => p.Product.ProductPhotos).OrderByDescending(prd => prd.Product.CreatedDate).Take(4)
                                                                                        .ToListAsync();


            model.CategoryId = categoryId;
            if (subcategoryId != null)
            {
                model.SubCategoryId = subcategoryId;
            }

            //model.SubcategoryLanguages = db.SubcategoryLanguages
            //                                .Where(sl => sl.LanguageId == langId && sl.Subcategory.CategoryId == categoryId)
            //                                .Include(sl => sl.Subcategory)
            //                                .ThenInclude(sl => sl.Category).ToList();

            //model.CategoryBrands = db.CategoryBrands.Where(b => b.CategoryId == categoryId).Include(cb => cb.Brand).ToList();
            model.Count = await db.ProductLanguages.Where(pl => pl.LanguageId == langId &&
                                                                       (subcategoryId != null ? pl.Product.SubcategoryId == subcategoryId : true
                                                                                && categoryId != null ? pl.Product.Subcategory.CategoryId == categoryId : true))
                                                                                        .CountAsync();
            model.ProductPhotos = db.ProductPhotos.Where(pp => pp.Product.Subcategory.CategoryId == categoryId)
                                                    .Include(pp => pp.Product)
                                                    .ThenInclude(pp => pp.Subcategory)
                                                    .ThenInclude(sc => sc.Category)
                                                    .ToList();

            

            //model.Colors = GetFilterByProduct(model.ProductLanguages, (x) =>
            //{
            //    var product_color_ids = db.ProductColors.Where(p => p.ProductId == x).Select(s => s.ColorId).ToList();
            //    return product_color_ids;
            //}, (x) =>
            //{
            //    var color = db.ColorLanguage.Where(c => c.ColorId == x && c.LanguageId == langId).Select(c => c.Name).FirstOrDefault();
            //    return color;
            //});

            //model.Sizes = GetFilterByProduct(model.ProductLanguages, (x) =>
            //{
            //    var product_size_ids = db.ProductSizes.Where(p => p.ProductId == x).Select(s => s.SizeId).ToList();
            //    return product_size_ids;
            //}, (x) =>
            //{
            //    var size = db.Sizes.Where(c => c.Id == x).Select(c => c.Name).FirstOrDefault();
            //    return size;
            //});

            return View(model);
        }


        //[HttpPost]
        //public async Task<JsonResult> Filter(FilterModel filter)
        //{
        //    int langId = HttpContext.GetLanguage("langId");

        //    List<ProductLanguage> productLanguages = await db.ProductLanguages.Where(pl => pl.LanguageId == langId &&
        //                                                                                (filter.SubcategoryId != 0 ? pl.Product.SubcategoryId == filter.SubcategoryId : false))
        //                                                                                    .Include(pl => pl.Product)
        //                                                                                        .ToListAsync();


        //    //filtered products
        //    List<ProductLanguage> filtered_products = new List<ProductLanguage>();


        //    if (filter.Brands != null)
        //    {
        //        foreach (var brand in filter.Brands)
        //        {
        //            List<Product> productBrands = await db.Products.Where(ps => ps.BrandId == brand && (filter.SubcategoryId != 0 ? ps.SubcategoryId == filter.SubcategoryId : true)).ToListAsync();
        //            if (productBrands.Count != 0)
        //            {
        //                //if subcategory null we need to make array by brands
        //                if (filter.SubcategoryId == 0)
        //                {
        //                    foreach (var productBrand in productBrands)
        //                    {
        //                        productLanguages.Add(await db.ProductLanguages.Where(pl => pl.ProductId == productBrand.Id).FirstOrDefaultAsync());
        //                    }

        //                    filtered_products = productLanguages;
        //                }
        //                else
        //                {
        //                    //if not we need to resize array by brand
        //                    foreach (var product in productLanguages)
        //                    {
        //                        if (productBrands.Select(pl => pl.Id).Contains(product.ProductId))
        //                        {
        //                            filtered_products.Add(product);
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    else if (filter.SubcategoryId != 0)
        //    {
        //        filtered_products = productLanguages;
        //    }

        //    //if boss of them null we need to make array by category
        //    if (filter.Brands == null && filter.SubcategoryId == 0)
        //    {
        //        List<ProductLanguage> productsbycategory = await db.ProductLanguages.Where(pl => pl.LanguageId == langId &&
        //                                                                                    pl.Product.Subcategory.CategoryId == filter.CategoryId)
        //                                                                                      .Include(pl => pl.Product)
        //                                                                                        .ToListAsync();

        //        filtered_products = productsbycategory;
        //    }

        //    if (filter.Sizes != null)
        //    {
        //        foreach (var size in filter.Sizes)
        //        {
        //            List<ProductSize> productSizes = await db.ProductSizes
        //                                                        .Where(ps => ps.Size.Name == size)
        //                                                                .ToListAsync();
        //            List<ProductLanguage> have_to_remove = new List<ProductLanguage>();


        //            if (productSizes.Count != 0)
        //            {
        //                //find products that you have to remove from array
        //                foreach (var product in filtered_products)
        //                {
        //                    if (!productSizes.Select(pl => pl.ProductId).Contains(product.ProductId))
        //                    {
        //                        have_to_remove.Add(product);
        //                    }
        //                }

        //                //remove products
        //                foreach (var item in have_to_remove)
        //                {
        //                    filtered_products.Remove(item);
        //                }

        //            }
        //        }
        //    }

        //    if (filter.Colors != null)
        //    {
        //        foreach (var color in filter.Colors)
        //        {
        //            var color_obj = db.ColorLanguage.Where(cl => cl.Name == color).FirstOrDefault();
        //            List<ProductColor> productColors = await db.ProductColors.Where(ps => ps.ColorId == color_obj.ColorId).ToListAsync();
        //            List<ProductLanguage> have_to_remove = new List<ProductLanguage>();
        //            if (productColors.Count != 0)
        //            {
        //                //find products that you have to remove from array
        //                foreach (var product in filtered_products)
        //                {
        //                    if (!productColors.Select(p => p.ProductId).Contains(product.ProductId))
        //                    {
        //                        have_to_remove.Add(product);
        //                    }
        //                }
        //                //remove products
        //                foreach (var item in have_to_remove)
        //                {
        //                    filtered_products.Remove(item);
        //                }

        //            }

        //        }
        //    }


        //    List<ProductPhoto> productPhotos = await db.ProductPhotos.Where(pp => (filter.SubcategoryId != 0 ? pp.Product.SubcategoryId == filter.SubcategoryId : true
        //                                             && filter.CategoryId != 0 ? pp.Product.Subcategory.CategoryId == filter.CategoryId : true))
        //                                           .Include(pp => pp.Product)
        //                                           .ThenInclude(pp => pp.Subcategory)
        //                                           .ThenInclude(sc => sc.Category)
        //                                           .ToListAsync();

        //    return Json(new
        //    {
        //        status = 200,
        //        data = new
        //        {
        //            filtered_products,
        //            productPhotos
        //        }
        //    });




        //}

        //public List<string> GetFilterByProduct(List<ProductLanguage> ProductLanguages, Func<int, List<int>> get_element_ids, Func<int, string> get_element_name)
        //{
        //    List<string> elements = new List<string>();
        //    foreach (var product in ProductLanguages)
        //    {
        //        var element_ids = get_element_ids(product.ProductId);

        //        if (element_ids.Count != 0)
        //        {
        //            foreach (var element_id in element_ids)
        //            {
        //                var element_name = get_element_name(element_id);

        //                if (!elements.Contains(element_name))
        //                {
        //                    elements.Add(element_name);
        //                }
        //            }
        //        }
        //    }
        //    return elements;
        //}

        public async Task<JsonResult> LoadMore(int? subcategoryId, int? categoryId)
        {

            int langId = HttpContext.GetLanguage("langId");
            if (subcategoryId == null && categoryId == null)
            {
                return Json(new
                {
                    status = 404
                });
            }
            if (HttpContext.Session.GetString("LoadMore") == null)
            {
                HttpContext.Session.SetString("LoadMore", "0");
            }
            int current = Convert.ToInt32(HttpContext.Session.GetString("LoadMore")) + 1;
            HttpContext.Session.SetString("LoadMore", current.ToString());

            var prds = await db.ProductLanguages.Where(pl =>
                                         pl.LanguageId == langId &&
                                         categoryId != null ? pl.Product.Subcategory.CategoryId == categoryId : true
                                         && subcategoryId != null ? pl.Product.SubcategoryId == subcategoryId : true
                                          )
                                        .Select(pl => new {
                                             Id=  pl.Product.Id,
                                             Price = pl.Product.Price,
                                             DiscountPercent= pl.Product.DiscountPercent,
                                             Subcategory= pl.Product.Subcategory,
                                             Name= pl.Name,
                                             ProductPhotos= pl.Product.ProductPhotos
                                        })
                                        .ToListAsync();


            //List<ProductPhoto> productPhotos = await db.ProductPhotos.Where(pp => pp.Product.Subcategory.CategoryId == categoryId)
            //                                        .ToListAsync();

            List<Card> WishlistCards = new List<Card>();
            if (HttpContext.Session.GetObjectFromJson<Card>("Wishlist") != null)
            {
                WishlistCards = HttpContext.Session.GetObjectFromJson<Card>("Wishlist");
            }



            return Json(new
            {
                status = 200,
                data = new
                {
                    list = prds.Skip(1 * current).Take(4),
                    total = prds.Count,
                    wishlistCards = WishlistCards
                }
            });
        }
    }
}