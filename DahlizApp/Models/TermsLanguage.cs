using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class TermsLanguage
    {
        public int Id { get; set; }
        public Terms Terms { get; set; }
        public int TermsId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
        public string Data { get; set; }
    }
}
