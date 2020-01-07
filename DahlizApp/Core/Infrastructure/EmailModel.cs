using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Core.Infrastructure
{
    public class EmailModel
    {
        public string toEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
