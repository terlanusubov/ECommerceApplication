using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models.PaymentModels
{
    public class CRespGetPaymentKey
    {
        public CStatus status { get; set; }
        public string paymentKey { get; set; }
    }
}
