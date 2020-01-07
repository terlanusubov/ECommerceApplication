using DahlizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class SubcategoryViewModel
    {
        public List<SubcategoryLanguage> subcategoryLanguages { get; set; }
        public List<CategoryLanguage> categoryLanguages { get; set; }
        public List<Language> Languages { get; set; }
    }
}
