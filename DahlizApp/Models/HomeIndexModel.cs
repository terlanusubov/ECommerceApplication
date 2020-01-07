using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class HomeIndexModel
    {
        public List<Slide> Slides { get; set; }
        public Setting Setting { get; set; }
        public List<CategoryLanguage> CategoryLanguages { get; set; }
        public List<SubcategoryLanguage> SubcategoryLanguages { get; set; }
        public List<Ad> Ads { get; set; }
        public List<CategoryBrand> CategoryBrands { get; set; }
        public NotiLanguage NotiLanguage { get; set; }
        public List<Card> Cards { get; set; }
        public List<Language> Languages { get; set; }
        public Language Language { get; set; }
    }
}
