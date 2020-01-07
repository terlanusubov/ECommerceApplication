using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Core.Extensions;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly DahlizDb db;
        public SliderController(DahlizDb _db)
        {
            db = _db;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Slider");
            int langId = HttpContext.GetLanguage("adminLangId");

            List<Slide> slides = await db.Slides.ToListAsync();
            return View(slides);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Slide model)
        {
            if (ModelState.IsValid)
            {
                Slide slide = new Slide()
                {
                    Text= model.Text,
                    Title = model.Title,
                    PhotoPath = model.PhotoPath
                };

                await db.Slides.AddAsync(slide);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Fill all blanks");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id != null)
            {
                Slide slide = await db.Slides
                                            .Where(s => s.Id == id)
                                                .FirstOrDefaultAsync();

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", "Sliders", slide.PhotoPath);
                if (PhotoUpload.HasPhoto(path))
                {
                    using (var file = System.IO.File.Open(path, FileMode.Open))
                    {
                        FileInfo info = new FileInfo(path);
                        IFormFile photo = new FormFile(file, 0, file.Length, file.Name, info.Name);
                        slide.Photo = photo;
                    }
                }

                return View(slide);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Slide model,int SlideId)
        {
            if (ModelState.IsValid)
            {
                var slider = await db.Slides.Where(s => s.Id == SlideId).FirstOrDefaultAsync();
                slider.Text = model.Text;

                if(model.PhotoPath!= null)
                {
                    slider.PhotoPath = model.PhotoPath;
                }

                slider.Title = model.Title;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();

            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? slideId)
        {
            if(slideId != null)
            {
                Slide slide = await db.Slides
                                      .Where(s => s.Id == slideId)
                                              .FirstOrDefaultAsync();

                db.Slides.Remove(slide);
                await db.SaveChangesAsync();

                //Photo Delete
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", "Sliders", slide.PhotoPath);
                if (PhotoUpload.HasPhoto(path))
                {
                    PhotoUpload.DeletePhoto(path);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

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