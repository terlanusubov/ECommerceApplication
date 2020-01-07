using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.ViewModels
{
    public class CategoryModel
    {
        public List<ProductLanguage> ProductLanguages { get; set; }
        public List<CategoryLanguage> CategoryLanguages { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        public List<CategoryBrand> CategoryBrands { get; set; }
        public List<ProductPhoto> ProductPhotos { get; set; }
        public List<SubcategoryLanguage> SubcategoryLanguages { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Sizes { get; set; }
        public int? SubCategoryId { get; set; }
        public int? CategoryId { get; set; }

        public int Count { get; set; }
    }
}
