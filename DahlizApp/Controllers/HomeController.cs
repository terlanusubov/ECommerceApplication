using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Data;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using DahlizApp.Models;
using DahlizApp.Models.ViewModels;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DahlizApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DahlizDb db;
        public HomeController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            int langId =  HttpContext.GetLanguage("langId");
            if(langId == 0)
            {
                return RedirectToAction("SetLanguage", "Language", new { culture = "en", returnUrl = "/" });
            }
            HomeModel model = new HomeModel();
            model.Ads = db.Ads.ToList();
            model.Slides = db.Slides.ToList();

           // string cities = String.Empty;
           // string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "cities.json");
           // using(StreamReader reader = new StreamReader(path))
           // {
           //    cities = reader.ReadToEnd();
           // }

           // string countries = String.Empty;
           // string pathCountry = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "countries.json");
           // using (StreamReader reader = new StreamReader(pathCountry))
           // {
           //     countries = reader.ReadToEnd();
           // }

           // List<City> citiesList = JsonConvert.DeserializeObject<List<City>>(cities);
           // List<Country> countriesList = JsonConvert.DeserializeObject<List<Country>>(countries);


           //await db.Countries.AddRangeAsync(countriesList);
           // await db.SaveChangesAsync();

           // await db.Cities.AddRangeAsync(citiesList);
           // await db.SaveChangesAsync();
            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
