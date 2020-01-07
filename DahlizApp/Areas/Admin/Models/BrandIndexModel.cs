using DahlizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class BrandIndexModel
    {
        public List<Brand> Brands { get; set; }
        public List<CategoryBrand> CategoryBrands { get; set; }
        public List<CategoryLanguage> CategoryLanguages { get; set; }

    }
}
