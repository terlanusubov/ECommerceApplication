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

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShippingsController : Controller
    {
        private readonly DahlizDb _context;

        public ShippingsController(DahlizDb context)
        {
            _context = context;
        }

        // GET: Admin/Shippings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shippings.ToListAsync());
        }

        // GET: Admin/Shippings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipping = await _context.Shippings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipping == null)
            {
                return NotFound();
            }

            return View(shipping);
        }

        // GET: Admin/Shippings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Shippings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shipping);
        }

        // GET: Admin/Shippings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipping = await _context.Shippings.FindAsync(id);
            if (shipping == null)
            {
                return NotFound();
            }
            return View(shipping);
        }

        // POST: Admin/Shippings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] Shipping shipping)
        {
            if (id != shipping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingExists(shipping.Id))
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
            return View(shipping);
        }

        // GET: Admin/Shippings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipping = await _context.Shippings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipping == null)
            {
                return NotFound();
            }

            return View(shipping);
        }

        // POST: Admin/Shippings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipping = await _context.Shippings.FindAsync(id);
            _context.Shippings.Remove(shipping);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippingExists(int id)
        {
            return _context.Shippings.Any(e => e.Id == id);
        }
    }
}
