using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Photo { get; set; }
        public decimal Price { get; set; } 
        public int SizeId { get; set; }
        public int ProductDbQuantity { get; set; }
        public string SizeName { get; set; }
        public int DiscountPercent { get; set; }
    }
}
