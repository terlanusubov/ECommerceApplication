using DahlizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Areas.Admin.Models
{
    public class PaymentModel
    {
        public List<Payment> Payments { get; set; }
        public List<ProductLanguage> ProductLanguages { get; set; }
    }
}
