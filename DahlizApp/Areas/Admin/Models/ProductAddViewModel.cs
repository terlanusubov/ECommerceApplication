using DahlizApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class ProductAddViewModel
    {
        public List<CategoryLanguage> categoryLanguages { get; set; }
        public List<Size> sizes { get; set; }
        public List<ColorLanguage> colors { get; set; }
        public List<Brand> brands { get; set; }
        public List<Language> languages { get; set; }
        public List<ProductLanguage> ProductLanguages { get; set; }
        public Product Product { get; set; }
    }
}
