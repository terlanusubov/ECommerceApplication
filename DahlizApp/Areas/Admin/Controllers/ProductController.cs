using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DahlizApp.Areas.Admin.Models;
using DahlizApp.Core.Extensions;
using DahlizApp.Core.Infrastructure;
using DahlizApp.Data;
using DahlizApp.Models;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Size = DahlizApp.Models.Size;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly DahlizDb db;
        public IConfiguration Configuration { get; }

        public ProductController(DahlizDb _db, IConfiguration configuration)
        {
            db = _db;
            Configuration = configuration;

        }
        public async Task<IActionResult> Index()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Product");
            int langId = HttpContext.GetLanguage("adminLangId");

            List<ProductLanguage> productLanguages = await db.ProductLanguages.Where(pl => pl.LanguageId == langId)
                                                                                    .Include(p=>p.Product)
                                                                                            .ThenInclude(p=>p.ProductPhotos)
                                                                                                .ToListAsync();

            List<SubcategoryLanguage> subcategoryLanguages = await db.SubcategoryLanguages.Where(sl => sl.LanguageId == langId).Include(s=>s.Subcategory).ToListAsync();


            ProductIndexModel model = new ProductIndexModel
            {
                ProductLanguages = productLanguages,
                SubcategoryLanguages = subcategoryLanguages
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Product");
            int langId = HttpContext.GetLanguage("adminLangId");
            ProductAddViewModel model = new ProductAddViewModel
            {
                categoryLanguages = db.CategoryLanguages
                                            .Where(cl => cl.LanguageId == langId)
                                                    .Include(cl => cl.Category)
                                                            .ToList(),
                sizes = db.Sizes.ToList(),

                languages = db.Languages.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SubCategoryId != 0 && model.CategoryId != 0)
                {
                    #region Product Static Properties Add
                    Product product = new Product();
                    product.CreatedDate = DateTime.Now;
                    product.DiscountPercent = Convert.ToInt32(model.DiscountPercent);
                    product.Price = Math.Ceiling(Convert.ToDecimal(model.Price));
                    product.SubcategoryId = model.SubCategoryId;
                    for(int i=0; i < model.Count.Count; i++)
                    {
                        product.Quantity += Convert.ToInt32(model.Count[i]);
                    }
                    //product.Quantity = Convert.ToInt32(model.Quantity);
                    db.Products.Add(product);
                    #endregion


                    List<Language> languages = db.Languages.ToList();
                    if (Checker.CheckList(model.Names) && Checker.CheckList(model.Sizes)
                        && Checker.CheckList(model.Count)
                        && Checker.CheckList(model.Descriptions) && Checker.CheckList(model.Photos))
                    {
                        #region Product Language Add
                        for (int i = 0; i < languages.Count; i++)
                        {
                            ProductLanguage productLanguage = new ProductLanguage();
                            productLanguage.LanguageId = languages[i].Id;
                            productLanguage.Name = model.Names[i];
                            productLanguage.ProductId = product.Id;
                            productLanguage.Description = model.Descriptions[i];
                            db.ProductLanguages.Add(productLanguage);
                        }
                        #endregion

                        #region Product Size Add
                        for (int i = 0; i < model.Sizes.Count; i++)
                        {
                            ProductSizeCount productSizeCount = new ProductSizeCount();
                            productSizeCount.ProductId = product.Id;
                            productSizeCount.SizeId = model.Sizes[i];
                            productSizeCount.Count = Convert.ToInt32(model.Count[i]);
                            db.ProductSizeCounts.Add(productSizeCount);
                        }

                        #endregion

                        //#region Product Color Add
                        //for (int i = 0; i < model.Colors.Count; i++)
                        //{
                        //    ProductColor productColor = new ProductColor();
                        //    productColor.ProductId = product.Id;
                        //    productColor.ColorId = model.Colors[i];
                        //    db.ProductColors.Add(productColor);
                        //}
                        //#endregion

                        #region Product Photo Add
                        for (int i = 0; i < model.Photos.Count; i++)
                        {
                            ProductPhoto productPhoto = new ProductPhoto();
                            productPhoto.Product = product;
                            productPhoto.PhotoPath = model.Photos[i];
                            db.ProductPhotos.Add(productPhoto);
                        }
                        #endregion

                        db.SaveChanges();
                        HttpContext.Session.SetString("productId", product.Id.ToString());
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Category or Subcategory not selected");
                        return RedirectToAction(nameof(Add));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Make sure that you fill all required blanks!");
                    return RedirectToAction(nameof(Add));
                }
            }
            else
            {
                ModelState.AddModelError("", "Make sure that you fill all required blanks!");
                return RedirectToAction(nameof(Add));
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Checker.CheckLangId(HttpContext, db, "adminLangId").Wait();
            HttpContext.SetCurrentPage("Index", "Product");
            int langId = HttpContext.GetLanguage("adminLangId");
            var product = await db.Products
                                        .Where(p => p.Id == id)
                                            .FirstOrDefaultAsync();


            //Find Product Photos
            var product_photos = await db.ProductPhotos.Where(pp => pp.Product == product).ToListAsync();
            foreach (var product_photo in product_photos)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", "Products", product_photo.PhotoPath);
                if (PhotoUpload.HasPhoto(path))
                {
                    using (var file = System.IO.File.Open(path, FileMode.Open))
                    {
                        FileInfo info = new FileInfo(path);
                        IFormFile photo = new FormFile(file, 0, file.Length, file.Name, info.Name);
                        product.Photos.Add(photo);
                    }
                }

            }

            #region Make ProductEditViewModel 
            ProductEditViewModel model = new ProductEditViewModel()
            {
                ProductLanguages = await db.ProductLanguages
                                                    .Where(pl => pl.ProductId == id)
                                                         .Include(pl => pl.Product)
                                                                .ThenInclude(pl => pl.Subcategory)
                                                                      .ToListAsync(),

                CategoryLanguages = await db.CategoryLanguages
                                            .Where(cl => cl.LanguageId == langId)
                                                    .Include(cl => cl.Category)
                                                            .ToListAsync(),
                SubcategoryLanguages = await db.SubcategoryLanguages
                                                .Where(sl => sl.LanguageId == langId)
                                                .Include(sl => sl.Subcategory)
                                                .ToListAsync(),

                Sizes = await db.Sizes.ToListAsync(),

                ProductSizeCounts = await db.ProductSizeCounts.Where(ps => ps.ProductId == product.Id).Include(ps => ps.Size).ToListAsync(),

                //Colors = await db.ColorLanguage
                //                            .Include(cl => cl.Color)
                //                                 .Where(cl => cl.LanguageId == langId)
                //                                    .ToListAsync(),
                ProductColors = await db.ProductColors
                                            .Where(pc => pc.ProductId == product.Id)
                                                .Include(pc => pc.Product)
                                                        .ToListAsync(),

                Languages = await db.Languages.ToListAsync(),

                Product = await db.Products
                                        .Where(p => p.Id == id)
                                            .FirstOrDefaultAsync(),
            };
            #endregion
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SubCategoryId != 0 && model.CategoryId != 0)
                {
                    #region Product Static Properties Edit
                    Product product = db.Products.Where(p => p.Id == model.ProductId).FirstOrDefault();

                    product.Price = Math.Ceiling(Convert.ToDecimal(model.Price));
                    product.SubcategoryId = model.SubCategoryId;
                    product.Quantity = 0;
                    product.DiscountPercent = Convert.ToInt32(model.DiscountPercent);
                    #endregion

                    List<ProductLanguage> productLanguages = await db.ProductLanguages
                                                                        .Where(pl => pl.ProductId == product.Id)
                                                                                    .ToListAsync();

                    List<ProductSizeCount> productSizeCounts = await db.ProductSizeCounts
                                                                .Where(ps => ps.ProductId == product.Id)
                                                                     .Include(ps => ps.Size)
                                                                        .ToListAsync();

                    List<ProductColor> productColors = await db.ProductColors
                                                                .Where(pc => pc.ProductId == product.Id)
                                                                        .Include(pc => pc.Color)
                                                                                .ToListAsync();

                    List<ProductPhoto> productPhotos = await db.ProductPhotos.Where(pp => pp.Product == product).ToListAsync();

                    if (Checker.CheckList(model.Names) && Checker.CheckList(model.Sizes)
                         && Checker.CheckList(model.Count)
                        && Checker.CheckList(model.Descriptions))
                    {
                        #region Product Language Edit
                        //Product Language Edit
                        for (int i = 0; i < productLanguages.Count; i++)
                        {
                            productLanguages[i].Name = model.Names[i];
                            productLanguages[i].Description = model.Descriptions[i];
                        }

                        #endregion



                        #region Product Size Edit
                        //Product Size Edit
                        if (model.Sizes.Count > productSizeCounts.Count)
                        {
                            for (int i = 0; i < model.Sizes.Count; i++)
                            {
                                if (i + 1 > productSizeCounts.Count)
                                {
                                    ProductSizeCount productSizeCount = new ProductSizeCount();
                                    productSizeCount.ProductId = product.Id;
                                    productSizeCount.SizeId = model.Sizes[i];
                                    productSizeCount.Count = Convert.ToInt32(model.Count[i]);
                                    product.Quantity += Convert.ToInt32(model.Count[i]);
                                    db.ProductSizeCounts.Add(productSizeCount);
                                }
                                else
                                {
                                    productSizeCounts[i].SizeId = model.Sizes[i];
                                }
                            }
                        }
                        else if (model.Sizes.Count == productSizeCounts.Count)
                        {
                            for (int i = 0; i < model.Sizes.Count; i++)
                            {
                                productSizeCounts[i].SizeId = model.Sizes[i];
                                productSizeCounts[i].Count = Convert.ToInt32(model.Count[i]);
                                product.Quantity += Convert.ToInt32(model.Count[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < productSizeCounts.Count; i++)
                            {
                                if (i + 1 <= model.Sizes.Count)
                                {
                                    productSizeCounts[i].SizeId = model.Sizes[i];
                                    productSizeCounts[i].Count = Convert.ToInt32(model.Count[i]);
                                    product.Quantity += Convert.ToInt32(model.Count[i]);
                                }
                                else
                                {
                                    ProductSizeCount productSizeCount = db.ProductSizeCounts
                                                                    .Where(ps => ps.SizeId == productSizeCounts[i].SizeId)
                                                                            .FirstOrDefault();
                                    db.ProductSizeCounts.Remove(productSizeCount);
                                }
                            }
                        }

                        #endregion


                        #region Photo Edit
                        if (model.DeletePhotos != null)
                        {
                            //then we have to delete old photos
                            foreach (var deletePhoto in model.DeletePhotos)
                            {
                                ProductPhoto productPhoto = await db.ProductPhotos
                                                                            .Where(pp => pp.Product == product && pp.PhotoPath == deletePhoto)
                                                                                .FirstOrDefaultAsync();
                                if(productPhoto!= null)
                                {
                                    db.ProductPhotos.Remove(productPhoto);
                                }
                            }
                        }
                        if (model.Photos != null)
                        {

                            //then we have to add new photos
                            foreach (var photo in model.Photos)
                            {
                                if(photo != null)
                                {
                                    if (!productPhotos.Select(p => p.PhotoPath).Contains(photo))
                                    {
                                        ProductPhoto productPhoto = new ProductPhoto();
                                        productPhoto.Product = product;
                                        productPhoto.PhotoPath = photo;
                                        db.ProductPhotos.Add(productPhoto);
                                    }
                                }
                            }
                        }
                        #endregion

                        await db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));

                    }
                    else
                    {
                        ModelState.AddModelError("", "Make Sure that you fill al required blanks!");
                        return RedirectToAction(nameof(Edit));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Category or Subcategory not selected");
                    return RedirectToAction(nameof(Edit));
                }
            }
            else
            {
                ModelState.AddModelError("", "Make Sure that you fill al required blanks!");
                return RedirectToAction(nameof(Edit));
            }

        }


        public JsonResult SubCategory(int? CategoryId, int langId)
        {
            if (CategoryId == null)
            {
                return Json(new
                {
                    status = 404
                });
            }
            List<SubcategoryLanguage> data = db.SubcategoryLanguages.Where(sl => sl.LanguageId == langId && sl.Subcategory.CategoryId == CategoryId)
                                                                    .Include(sl => sl.Subcategory)
                                                                    .ToList();
            return Json(new
            {
                data = data,
                status = 200
            });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? productId)
        {
            if (productId != null)
            {
                Product product = await db.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
                List<ProductPhoto> productPhotos = await db.ProductPhotos.Where(pp => pp.Product == product).ToListAsync();
                List<ProductLanguage> productLanguages = await db.ProductLanguages.Where(pl => pl.ProductId == productId).ToListAsync();
                List<ProductSize> productSizes = await db.ProductSizes.Where(ps => ps.ProductId == productId).ToListAsync();

                db.ProductPhotos.RemoveRange(productPhotos);
                db.ProductSizes.RemoveRange(productSizes);
                db.ProductLanguages.RemoveRange(productLanguages);
                db.Products.Remove(product);

                await db.SaveChangesAsync();

                //DELET PHOTOS FROM UPLOADS FOLDER
                foreach (var productPhoto in productPhotos)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", "Products", productPhoto.PhotoPath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Product is not exsists");
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
                        string thumbpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", "Thumbnails",filename);
                        if (PhotoUpload.HasPhoto(_path))
                        {
                            Guid guid = new Guid();
                            _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", path, guid+filename);
                            thumbpath= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Uploads", "Thumbnails",guid + filename);
                        }

                        PhotoUpload.UploadTo(Photos[i], _path);

                        #region Resize Image
                        //using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(_path))
                        //{
                        //    image.Mutate(x =>
                        //    {
                        //        x
                        //        .Resize(168, 168);
                        //    });
                        //    image.Save(thumbpath);
                        //}
                        #endregion

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
        public JsonResult RemoveFile(string filename,string path)
        {
            if(filename != null && path != null)
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

        public static ImageFormat ParseImageFormat(string str)
        {
            return (ImageFormat)typeof(ImageFormat)
                    .GetProperty(str, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .GetValue(null);
        }

        public async Task<JsonResult> FillSize()
        {
            List<Size> sizes = await db.Sizes.ToListAsync();
            return Json(new
            {
                status = 200,
                data = sizes
            });
        }
    }
}