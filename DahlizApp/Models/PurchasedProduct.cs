using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class PurchasedProduct
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
