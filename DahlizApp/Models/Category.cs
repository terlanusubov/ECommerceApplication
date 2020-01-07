using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public List<Subcategory> Subcategories { get; set; }
    }
}
