using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.ViewModels
{
    public class StepTwoRegisterModel
    {
        [Required]
        public string Phone { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        public string Address { get; set; }
        [Required]
        public string Post { get; set; }
    }
}
