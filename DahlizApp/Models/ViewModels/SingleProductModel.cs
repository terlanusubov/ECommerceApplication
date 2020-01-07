using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.ViewModels
{
    public class SingleProductModel
    {
        public List<ProductPhoto> ProductPhotos { get; set; }
        public List<ProductSizeCount> ProductSizeCount { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<ColorLanguage> ColorLanguages { get; set; }
        public List<ProductReview> ProductReviews { get; set; }
        public ProductLanguage ProductLanguage { get; set; }
        public Product Product { get; set; }
    }
}
