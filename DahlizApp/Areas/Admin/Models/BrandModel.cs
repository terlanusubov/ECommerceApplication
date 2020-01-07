using DahlizApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class BrandModel
    {
        [Required]
        public string BrandName { get; set; }
        public List<int> Categories { get; set; }
        public int BrandId { get; set; }

        public List<CategoryBrand> CategoryBrands { get; set; }
        public List<CategoryLanguage> CategoryLanguages{ get; set; }
        public Brand Brand { get; set; }
    }
}
