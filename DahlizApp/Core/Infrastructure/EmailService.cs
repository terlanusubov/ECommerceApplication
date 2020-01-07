using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DahlizApp.Core.Infrastructure
{
    public class EmailService
    {
        private readonly EmailServiceOption _option;
        public EmailService(IOptions<EmailServiceOption> option)
        {
            _option = option.Value;
        }
        public async Task<int> SendMailAsync(string toEmail, string Subject, string Message, string FilePath = null)
        {
            using (MailMessage msg = new MailMessage())
            {
                using (SmtpClient connect = new SmtpClient(_option.Host, _option.Port))
                {
                    msg.From = new MailAddress(_option.Email, _option.DisplayName, System.Text.Encoding.UTF8);
                    msg.To.Add(toEmail);
                    msg.Subject = Subject;
                    msg.Body = Message;
                    msg.IsBodyHtml = true;
                    if (FilePath != null)
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(FilePath);
                        msg.Attachments.Add(attachment);
                        attachment.ContentId = "imgContentId";
                    }
                    connect.Credentials = new NetworkCredential(_option.Email, _option.Password);
                    connect.EnableSsl = _option.EnableSSL;
                    await connect.SendMailAsync(msg);
                    return Convert.ToInt32(msg.DeliveryNotificationOptions);
                }
            }
        }
    }

    public class EmailServiceOption
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool EnableSSL { get; set; }
    }
}
