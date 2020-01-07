using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class SubcategoryLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Subcategory Subcategory { get; set; }
        public int SubcategoryId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
    }
}
