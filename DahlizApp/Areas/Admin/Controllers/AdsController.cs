using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Http;
using DahlizApp.Core.Infrastructure;
using System.IO;
using DahlizApp.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdsController : Controller
    {
        private readonly DahlizDb _context;

        public AdsController(DahlizDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, _context, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Ads");
            int langId = HttpContext.GetLanguage("adminLangId");
            return View(await _context.Ads.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhotoPath,Heading,Title,Link")] Ad ad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ad);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhotoPath,Heading,Title,Link")] Ad ad)
        {
            if (id != ad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdExists(ad.Id))
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
            return View(ad);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Admin", "Uploads", "Ads", ad.PhotoPath);
            PhotoUpload.DeletePhoto(path);

            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(int id)
        {
            return _context.Ads.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult AddPhoto(string path)
        {
            List<IFormFile> Photos = Request.Form.Files.ToList();
            var photo_info = new List<object>();
            if (Checker.CheckList(Photos) && path != null)
            {
                for (int i = 0; i < Photos.Count; i++)
                {
                    if (PhotoUpload.IsValid(Photos[i]))
                    {
                        string filename = (DateTime.Now.ToShortDateString() + Photos[i].FileName).Replace(" ", "").Replace("/", "");
                        string _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", path, filename);
                        if (PhotoUpload.HasPhoto(_path))
                        {
                            Guid guid = new Guid();
                            _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", path, guid + filename);
                        }
                        PhotoUpload.UploadTo(Photos[i], _path);
                        var obj = new
                        {
                            Filename = filename,
                            Url = $"/Admin/Uploads/{path}/" + filename
                        };
                        photo_info.Add(obj);
                    }
                    else
                    {
                        return Json(new
                        {
                            response = 400
                        });
                    }
                }
                return Json(new
                {
                    status = 200,
                    data = photo_info
                });
            }
            else
            {
                return Json(new { error = "something went wrong" });
            }
        }
        [HttpPost]
        public JsonResult RemoveFile(string filename, string path)
        {
            if (filename != null && path != null)
            {
                string _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", path, filename);
                if (System.IO.File.Exists(_path))
                {
                    System.IO.File.Delete(_path);
                }
                return Json(new
                {
                    status = 200
                });
            }
            else
            {
                return Json(new
                {
                    status = 400
                });
            }
        }
    }
}
