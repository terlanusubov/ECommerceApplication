using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Country Country { get; set; }

        public int CountryId { get; set; }
    }
}
