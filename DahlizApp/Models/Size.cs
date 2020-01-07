using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Size
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
