using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Controllers
{
    public class CardController : Controller
    {
        private readonly DahlizDb db;
        public CardController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated && HttpContext.Session.GetString("user_id") != null)
            {
                string user_id = HttpContext.Session.GetString("user_id");
                User user = await db.Users.Where(u => u.Id == user_id).FirstOrDefaultAsync();
                List<Card> Cards = new List<Card>();
                if (HttpContext.Session.GetObjectFromJson<Card>("Card") != null)
                {
                    Cards = HttpContext.Session.GetObjectFromJson<Card>("Card");
                }

                CardModel cardModel = new CardModel();
                cardModel.Shippings = await db.Shippings.ToListAsync();
                cardModel.Cards = Cards;
                cardModel.user = user;
                return View(cardModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        
        }
        public JsonResult Add(int id, int quantity, int sizeId)
        {
            int langId = HttpContext.GetLanguage("langId");

            ProductLanguage prd = db.ProductLanguages.Where(pl => pl.ProductId == id && pl.LanguageId == langId)
                .Include(pl => pl.Product).FirstOrDefault();
            if (prd == null)
            {
                return Json(new
                {
                    status = 404
                });
            }

            if (HttpContext.Session.GetString("Card") == null)
            {
                HttpContext.Session.SetObjectAsJson("Card", new List<Card>());
            }

            List<Card> Carts = HttpContext.Session.GetObjectFromJson<Card>("Card") as List<Card>;

            if (Carts.Find(c => c.Id == prd.ProductId && c.SizeId == sizeId) != null)
            {
                Carts.Find(c => c.Id == prd.ProductId).Quantity += quantity;
            }
            else
            {
                Card card = new Card
                {
                    Id = prd.ProductId,
                    Quantity = quantity,
                    Name = prd.Name,
                    Price = prd.Product.DiscountPercent != 0 ? (decimal)(prd.Product.Price - (prd.Product.Price * prd.Product.DiscountPercent / 100)) : prd.Product.Price,
                    Photo = "/Admin/Uploads/Products/" + db.ProductPhotos.Where(p => p.ProductId == prd.ProductId).FirstOrDefault().PhotoPath,
                    ProductDbQuantity = prd.Product.Quantity,
                    SizeId = sizeId,
                    SizeName = db.Sizes.Where(s => s.Id == sizeId).FirstOrDefault().Name
                };
                Carts.Add(card);
            }

            HttpContext.Session.SetObjectAsJson("Card", Carts);

            return Json(new
            {
                status = 200,
                data = new
                {
                    count = Carts.Sum(c => c.Quantity),
                    total = Carts.Sum(c => c.Quantity * c.Price).ToString(),
                    list = Carts
                }
            });
        }

        public JsonResult Remove(int id, int sizeId)
        {
            int langId = HttpContext.GetLanguage("langId");

            ProductLanguage prd = db.ProductLanguages.Where(pl => pl.ProductId == id && pl.LanguageId == langId)
                .Include(pl => pl.Product).FirstOrDefault();
            if (prd == null)
            {
                return Json(new
                {
                    status = 404
                });
            }

            if (HttpContext.Session.GetString("Card") == null)
            {
                return Json(new
                {
                    status = 405
                });
            }

            List<Card> Carts = HttpContext.Session.GetObjectFromJson<Card>("Card") as List<Card>;

            if (Carts.Find(c => c.Id == prd.ProductId && c.SizeId == sizeId) == null)
            {
                return Json(new
                {
                    status = 406
                });
            }

            Carts.Remove(Carts.Find(c => c.Id == prd.ProductId));

            HttpContext.Session.SetObjectAsJson("Card", Carts);

            return Json(new
            {
                status = 200,
                data = new
                {
                    count = Carts.Sum(c => c.Quantity),
                    total = Carts.Sum(c => c.Quantity * c.Price).ToString(),
                    list = Carts
                }
            });
        }

        public JsonResult ChangeQty(int id, int qty, int sizeId,string key)
        {
            int langId = HttpContext.GetLanguage("langId");
            decimal shipPrice;
            if(key != "0" && key!=null)
            {
                shipPrice = db.Shippings.Where(s => s.Name == key).FirstOrDefault().Price;
            }
            else
            {
                shipPrice = 0;
            }
            ProductLanguage prd = db.ProductLanguages.Where(pl => pl.ProductId == id && pl.LanguageId == langId)
                  .Include(pl => pl.Product).FirstOrDefault();
            if (prd == null)
            {
                return Json(new
                {
                    status = 404
                });
            }
            if (HttpContext.Session.GetString("Card") == null)
            {
                return Json(new
                {
                    status = 405
                });
            }

            List<Card> Carts = HttpContext.Session.GetObjectFromJson<Card>("Card") as List<Card>;

            if (Carts.Find(c => c.Id == prd.ProductId && c.SizeId == sizeId) == null)
            {
                return Json(new
                {
                    status = 406
                });
            }

            if (HttpContext.Session.GetString("Total") != null)
            {
                var total = Convert.ToDecimal(HttpContext.Session.GetString("Total"));
                var oldPrice = Carts.Find(c => c.Id == prd.ProductId).Quantity * Carts.Find(c => c.Id == prd.ProductId).Price;
                var shipping = total - Carts.Sum(c => c.Quantity * c.Price);
                Carts.Find(c => c.Id == prd.ProductId && c.SizeId == sizeId).Quantity = qty;
                var newPrice = Carts.Find(c => c.Id == prd.ProductId).Quantity * Carts.Find(c => c.Id == prd.ProductId).Price;
              
                var newTotal = newPrice + shipPrice;
                HttpContext.Session.SetString("Total", newTotal.ToString());
                return Json(new
                {
                    status = 200,
                    data = new
                    {
                        count = Carts.Sum(c => c.Quantity),
                        subtotal = (Carts.Find(c => c.Id == prd.ProductId).Quantity * Carts.Find(c => c.Id == prd.ProductId).Price).ToString(),
                        total = newTotal,
                        list = Carts
                    }
                });
            }

            Carts.Find(c => c.Id == prd.ProductId && c.SizeId == sizeId).Quantity = qty;


            HttpContext.Session.SetObjectAsJson("Card", Carts);

            return Json(new
            {
                status = 200,
                data = new
                {
                    count = Carts.Sum(c => c.Quantity),
                    subtotal = (Carts.Find(c => c.Id == prd.ProductId).Quantity * Carts.Find(c => c.Id == prd.ProductId).Price).ToString(),
                    total = Carts.Sum(c=>c.Quantity*c.Price),
                    list = Carts
                }
            });
        }
        public JsonResult ChangeShippingPrice(string key)
        {
            if (key != null)
            {
                Shipping shipping = db.Shippings.Where(s=>s.Name == key).FirstOrDefault();

           
                    var prices = HttpContext.Session.GetObjectFromJson<Card>("Card").Sum(c => c.Quantity * c.Price);
                    HttpContext.Session.SetString("Total", (prices + shipping.Price).ToString());


                return Json(new {
                    
                    shipping = shipping.Price,
                    total = HttpContext.Session.GetString("Total")
                });

            }
            else
            {
                return Json(new
                {
                    status = 404
                });
            }
        }

        [HttpPost]
        public JsonResult GetTotalPrice()
        {
            return Json(new
            {
                total =HttpContext.Session.GetObjectFromJson<Card>("Card").Sum(c => c.Quantity * c.Price)
            });
        }


    }
}