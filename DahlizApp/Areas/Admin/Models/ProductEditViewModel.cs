using DahlizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class ProductEditViewModel
    {
        public List<CategoryLanguage> CategoryLanguages { get; set; }
        public List<Size> Sizes { get; set; }
        public List<ProductSizeCount> ProductSizeCounts { get; set; }
        public List<ColorLanguage> Colors { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Language> Languages { get; set; }
        public List<ProductLanguage> ProductLanguages { get; set; }
        public List<SubcategoryLanguage> SubcategoryLanguages { get; set; }
        public Product Product { get; set; }
    }
}
