using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.PaymentModels
{
    public class CPaymentItem
    {
        public string merchantName { get; set; }
        public int amount { get; set; }
        public string lang { get; set; }
        public string cardType { get; set; }
        public string description { get; set; }
        public string hashCode { get; set; }

    }
}
