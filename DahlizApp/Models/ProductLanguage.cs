using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class ProductLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
    }
}
