using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Ad
    {
        public int Id { get; set; }
        [Required]
        public string PhotoPath { get; set; }
        [Required]
        public string Heading { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Link { get; set; }


        [NotMapped]
        public IFormFile Photo { get; set; }
        [NotMapped]
        public string  DeletePhotoPath { get; set; }
    }
}
