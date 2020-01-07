using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.ViewModels
{
    public class FilterModel
    {
        public List<int> Brands { get; set; }
        public int SubcategoryId { get; set; }
        public string PriceMax { get; set; }
        public string PriceMin { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Sizes { get; set; }
        public int CategoryId { get; set; }
    }
}
