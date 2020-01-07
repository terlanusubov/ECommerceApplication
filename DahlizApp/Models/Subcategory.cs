using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Subcategory
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }
}
