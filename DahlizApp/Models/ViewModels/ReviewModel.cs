using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.ViewModels
{
    public class ReviewModel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public int Rate { get; set; }
    }
}
