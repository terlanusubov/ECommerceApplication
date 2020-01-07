using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string ProductName { get; set; }
        public int? ProductDiscount { get; set; }
        public int ProductId { get; set; }
        public decimal ProductPrice  { get; set; }
        public string ProductSize { get; set; }
        public DateTime Date { get; set; }
        public string PaymentKey { get; set; }
        public CardType CardType { get; set; }
        public int CardTypeId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Count { get; set; }
        public int Status { get; set; }

        public decimal ShippingPrice { get; set; }
    }
}
