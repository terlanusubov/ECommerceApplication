using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Product
    {
        public Product()
        {
            Photos = new HashSet<IFormFile>();
        }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public Subcategory Subcategory { get; set; }
        public int SubcategoryId { get; set; }
        //public Brand Brand { get; set; }
        //public int BrandId { get; set; }
        public int Rate { get; set; }

        public List<ProductPhoto> ProductPhotos { get; set; }

        [NotMapped]
        public ICollection<IFormFile> Photos { get; set; }
    }
}
