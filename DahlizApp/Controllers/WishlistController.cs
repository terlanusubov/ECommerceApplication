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
    public class WishlistController : Controller
    {
        private readonly DahlizDb db;
        public WishlistController(DahlizDb _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            List<Card> WishlistCards = new List<Card>();
            if (HttpContext.Session.GetObjectFromJson<Card>("Wishlist")!=null)
            {
                WishlistCards = HttpContext.Session.GetObjectFromJson<Card>("Wishlist");
            }
            else
            {
                WishlistCards = new List<Card>();
            }
            return View(WishlistCards);
        }

        public JsonResult Add(int id)
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

            if (HttpContext.Session.GetString("Wishlist") == null)
            {
                HttpContext.Session.SetObjectAsJson("Wishlist", new List<Card>());
            }

            List<Card> Wishlist = HttpContext.Session.GetObjectFromJson<Card>("Wishlist") as List<Card>;

            if (Wishlist.Find(c => c.Id == prd.ProductId) != null)
            {
               Wishlist.Remove(Wishlist.Find(c => c.Id == prd.ProductId));
            }
            else
            {
                Card wishlistCard = new Card
                {
                    Id = prd.ProductId,
                    Name = prd.Name,
                    Price = prd.Product.Price,
                    Photo = "/Admin/Uploads/Products/" + db.ProductPhotos.Where(p => p.ProductId == prd.ProductId).FirstOrDefault().PhotoPath,
                    ProductDbQuantity = prd.Product.Quantity,
                    DiscountPercent = prd.Product.DiscountPercent
                };
                Wishlist.Add(wishlistCard);
            }
            HttpContext.Session.SetObjectAsJson("Wishlist", Wishlist);

            return Json(new
            {
                status = 200,
                list = Wishlist,
            });

        }

        public JsonResult Remove(int id)
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

            if (HttpContext.Session.GetString("Wishlist") == null)
            {
                return Json(new
                {
                    status = 405
                });
            }
            List<Card> WishListCards = HttpContext.Session.GetObjectFromJson<Card>("Wishlist") as List<Card>;

            if(WishListCards.Find(w=>w.Id == id) == null)
            {
                return Json(new
                {
                    status = 406
                });
            }
            WishListCards.Remove(WishListCards.Find(c => c.Id == prd.ProductId));

            HttpContext.Session.SetObjectAsJson("Wishlist", WishListCards);

            return Json(new
            {
                status = 200
            });
        }
    }
}