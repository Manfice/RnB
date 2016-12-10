using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public static class PassAuth
    {
        public static async Task<string> SendMyMailAsync(string body, string to, string subject)
        {
            var fromAddress = new MailAddress("no-replay@redblackclub.ru", "Красное & Черное");
            var smtp = new SmtpClient
            {
                Host = "redblackclub.ru",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "^*J_ewv@"),//^*J_ewv@
                
            };
            ServicePointManager.ServerCertificateValidationCallback =
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
        public static string SendMyMail(string body, string to, string subject)
        {
            var fromAddress = new MailAddress("no-replay@redblackclub.ru", "Красное & Черное");
            var smtp = new SmtpClient
            {
                Host = "redblackclub.ru",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "^*J_ewv@"),//^*J_ewv@

            };
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) => true;
            using (var message = new MailMessage(fromAddress, new MailAddress(to))
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
            return $"Письмо отправленно на адрес: {to}";
        }
        public static string EncodeMd5(string s)
        {
            var md5 = MD5.Create();
            var inputButes = Encoding.UTF8.GetBytes(s);
            var hash = md5.ComputeHash(inputButes);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}