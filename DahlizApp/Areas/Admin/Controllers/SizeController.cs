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
    public class SizeController : Controller
    {
        private readonly DahlizDb db;
        public SizeController(DahlizDb _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();

            HttpContext.SetCurrentPage("index", "size");

            List<Size> sizes = await db.Sizes.ToListAsync();
            return View(sizes);
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Size size)
        {
            if (ModelState.IsValid)
            {
                db.Sizes.Add(size);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Name is required");
                return View(size);
            }
         
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            Size size = await db.Sizes.FindAsync(id);

            if (size == null)
            {
                return RedirectToAction("error", "home");
            }
            return View(size);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Size size)
        {
            if (size.Name == null)
            {
                ModelState.AddModelError("", "Name is required");
                return View(size);
            }

            db.Entry(size).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            Size size = await db.Sizes.FindAsync(id);
            if (size == null)
            {
                return RedirectToAction("error", "home");
            }
            else
            {
                db.Sizes.Remove(size);
                await db.SaveChangesAsync();
                return RedirectToAction("index", "size");
            }
        }
    }
}