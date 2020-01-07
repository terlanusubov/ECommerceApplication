using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class ColorLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int ColorId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
    }
}
