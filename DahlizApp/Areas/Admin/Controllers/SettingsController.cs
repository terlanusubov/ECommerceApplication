using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Authorization;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Core.Extensions;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly DahlizDb _context;

        public SettingsController(DahlizDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, _context, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Settings");
            int langId = HttpContext.GetLanguage("adminLangId");

            return View(await _context.Settings.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            Checker.CheckLangId(HttpContext, _context, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Details", "Settings");
            int langId = HttpContext.GetLanguage("adminLangId");

            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        public IActionResult Create()
        {
            Checker.CheckLangId(HttpContext, _context, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Create", "Settings");
            int langId = HttpContext.GetLanguage("adminLangId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Logo,Phone,Email,Description,Facebook,Twitter,Instagram")] Setting setting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            Checker.CheckLangId(HttpContext, _context, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Edit", "Settings");
            int langId = HttpContext.GetLanguage("adminLangId");
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Logo,Phone,Email,Description,Facebook,Twitter,Instagram")] Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setting = await _context.Settings.FindAsync(id);
            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
