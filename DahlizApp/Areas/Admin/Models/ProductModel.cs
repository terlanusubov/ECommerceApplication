using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DahlizApp.Areas.Admin.Models
{
    public class ProductModel
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int SubCategoryId { get; set; }
        public List<string> Names { get; set; }
        [Required]
        public string Price { get; set; }
        public string DiscountPercent { get; set; }
        [Required]
        public List<string> Count { get; set; }
        public List<int> Sizes { get; set; }
        public List<int> Colors { get; set; }
        public List<string> Descriptions { get; set; }
        public List<string> Photos { get; set; }
        //For Product Edit
        public int ProductId { get; set; }
        public List<string> DeletePhotos { get; set; }
    }
}
