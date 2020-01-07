using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Areas.Admin.Models;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Core.Infrastructure;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles= "Admin")]
    public class PaymentController : Controller
    {
        private readonly DahlizDb db;
        public PaymentController(DahlizDb _Db)
        {
            db = _Db;
        }
        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Payment");
            int langId = HttpContext.GetLanguage("adminLangId");

            List<ProductLanguage> productLanguages = await db.ProductLanguages
                                                                .Include(p => p.Product)
                                                                     .Where(p=>p.LanguageId == langId)
                                                                        .ToListAsync();

            List<Payment> payments = await db.Payments.OrderByDescending(p=>p.Date)
                          .ToListAsync();

            PaymentModel model = new PaymentModel()
            {
                ProductLanguages = productLanguages,
                Payments = payments
            };
            return View(model);
        }
    }
}