using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class AboutLanguage
    {
        public int Id { get; set; }
        public About About { get; set; }
        public int AboutId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
        public string Text { get; set; }
    }
}
