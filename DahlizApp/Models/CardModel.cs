using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Models
{
    public class CardModel
    {
        public List<Card> Cards { get; set; }
        public List<Shipping> Shippings { get; set; }
        public User user { get; set; }
    }
}
