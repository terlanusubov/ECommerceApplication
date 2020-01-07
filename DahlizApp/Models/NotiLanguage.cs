using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class NotiLanguage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Noti Noti { get; set; }
        public int NotiId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
    }
}
