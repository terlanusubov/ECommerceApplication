using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.PaymentModels
{
    public class CRespGetPaymentResult
    {
        public CStatus status { get; set; }
        public string paymentKey { get; set; }
        public string merchantName { get; set; }
        public decimal amount { get; set; }
        public int checkCount { get; set; }
        public string paymentDate { get; set; }
        public string cardNumber { get; set; }
        public string language { get; set; }
        public
            string description { get; set; }
        public string rrn { get; set; }

    }
}
