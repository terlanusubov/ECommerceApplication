using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Data;
using DahlizApp.Models;
using DahlizApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace DahlizApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly DahlizDb db;
        public ProductController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if(id != null)
            {
                int langId = HttpContext.GetLanguage("langId");
                Product product = await db.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
                product.Rate = await CalcProductRateAsync(product);
                SingleProductModel model = new SingleProductModel()
                {
                    ProductLanguage = await db.ProductLanguages
                                                .Where(pl => pl.ProductId == id && pl.LanguageId == langId)
                                                        .FirstOrDefaultAsync(),

                    //ProductColors = await db.ProductColors
                    //                                .Where(pc => pc.ProductId == id)
                    //                                        .Include(cl => cl.Color)
                    //                                                .ToListAsync(),

                    ProductSizeCount = await db.ProductSizeCounts
                                                    .Where(ps => ps.ProductId == id)
                                                            .Include(ps => ps.Size)
                                                                    .ToListAsync(),
                    ProductPhotos = await db.ProductPhotos
                                                    .Where(pp => pp.Product == product)
                                                            .ToListAsync(),
                    Product = product,
                    ProductReviews = await db.ProductReviews
                                                    .Where(pr => pr.ProductId == product.Id)
                                                       .Include(pr=>pr.User)
                                                            .ToListAsync(),

                    ColorLanguages = await db.ColorLanguage
                                                    .Where(cl => cl.LanguageId == langId)
                                                                .Include(cl=>cl.Color)
                                                                        .ToListAsync()
                };

                HttpContext.Session.SetString("product_id", id.ToString());
                return View(model);

            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        public async Task<JsonResult> Review(ReviewModel model)
        {
            if (ModelState.IsValid)
            {
                string productId = HttpContext.Session.GetString("product_id");
                User user = await db.Users.Where(u => u.Id == HttpContext.GetUserInfoFromSession("user_id")).FirstOrDefaultAsync();
                if(user != null && productId != null)
                {
                    ProductReview productReview = new ProductReview();
                    productReview.User = user;
                    productReview.Desc = model.Text;
                    productReview.CreatedDate = DateTime.Now;
                    productReview.ProductId = Convert.ToInt32(productId);
                    productReview.Rating = Convert.ToInt32(Convert.ToDouble(model.Rate));
                    productReview.Status = false;

                    db.ProductReviews.Add(productReview);
                    db.SaveChanges();
                    return Json(new
                    {
                        status=200,
                        data= productReview
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = 400,
                        error="User is not logged in"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    status = 400,
                    error = "Modelstate is not valid"
                });
            }
        }
        public async Task<int> CalcProductRateAsync(Product product)
        {
            List<ProductReview> productReviews = await db.ProductReviews
                                                                .Where(pr => pr.ProductId == product.Id)
                                                                        .ToListAsync();

            List<int> productRates = new List<int>();
            foreach (var productReview in productReviews)
            {
                productRates.Add(productReview.Rating);
            }

            int rate = 0;
            if (productRates.Count != 0)
            {
                rate = Convert.ToInt32(Math.Ceiling(productRates.Average())); 
            }

            return rate;
        }

        [HttpPost]
        public JsonResult FillQuantity(int sizeId,int id)
        {
            ProductSizeCount productSizeCount = db.ProductSizeCounts.Where(psc => psc.ProductId == id && psc.SizeId == sizeId).FirstOrDefault();
            if (productSizeCount == null)
            {
                return Json(new
                {
                    status = 404
                });
            }
            int count = productSizeCount.Count;
            return Json(new
            {
                status = 200,
                data = count
            });
        }
    }
}