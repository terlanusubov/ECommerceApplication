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

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly DahlizDb db;
        public HomeController(DahlizDb _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            string domainName = HttpContext.Request.Scheme;
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();

            HttpContext.SetCurrentPage("index","Home");
            return View();
        }
        public async Task<IActionResult> SetLanguage(string culture)
        {
           
            await HttpContext.SetLanguage(culture, db, "adminLangId");
            return RedirectToAction(HttpContext.GetCurrentPageAction(), HttpContext.GetCurrentPageController());
        }
    }
}