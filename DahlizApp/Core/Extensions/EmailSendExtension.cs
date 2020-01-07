using DahlizApp.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Core.Extensions
{
    public static class EmailSendExtension
    {
        public static string GetEmailTemplate(this EmailService service,string template_path,Func<string,string> replace)
        {
            string htmlBody = String.Empty;
            using(StreamReader reader = new StreamReader(template_path))
            {
                htmlBody = reader.ReadToEnd();
            }
            var message = replace(htmlBody);
            return message;
        }
    }
}
