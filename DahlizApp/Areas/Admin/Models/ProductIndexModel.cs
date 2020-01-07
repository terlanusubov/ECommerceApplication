using DahlizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class ProductIndexModel
    {
        public List<ProductLanguage> ProductLanguages { get; set; }
        public List<SubcategoryLanguage> SubcategoryLanguages { get; set; }
    }
}
