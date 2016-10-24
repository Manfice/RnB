using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public static class PassAuth
    {
        public static async Task<string> SendMyMailAsync(string body, string to, string subject)
        {
            var fromAddress = new MailAddress("admin@id-racks.ru", "Красное & Черное");
            var smtp = new SmtpClient
            {
                Host = "smtp.yandex.ru",
                Port = 25,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "1q2w3eZX")
            };
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) => true;
            using (var message = new MailMessage(fromAddress, new MailAddress(to))
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
            return $"Письмо отправленно на адрес: {to}";
        }
    }
}