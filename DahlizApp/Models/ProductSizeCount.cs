using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class ProductSizeCount
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public Size Size { get; set; }
        public int SizeId { get; set; }

        public int Count { get; set; }
    }
}
