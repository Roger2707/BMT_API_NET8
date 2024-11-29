using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace Store_API.Services
{
    public class EmailSenderService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com", //or another email sender provider
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("huynglesongtan8889@gmail.com", "qyra jbjz gxyj xihj")
            };

            return client.SendMailAsync("huynglesongtan8889@gmail.com", email, subject, htmlMessage);
        }
    }
}
