using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string PhotoPath { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        [NotMapped]
        public string DeletePhotoPath { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
