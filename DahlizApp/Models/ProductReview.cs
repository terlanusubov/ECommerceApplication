using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public string Desc { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Rating { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public bool Status { get; set; }
    }
}
