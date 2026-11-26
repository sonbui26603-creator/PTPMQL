using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace DemoMVC.Services
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;

        public SmtpEmailSender(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(
            string recipient,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(_options.SmtpHost) ||
                string.IsNullOrWhiteSpace(_options.FromAddress))
            {
                throw new InvalidOperationException(
                    "Cấu hình Email:SmtpHost và Email:FromAddress trước khi gửi email.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.FromAddress, _options.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(recipient));

            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                EnableSsl = _options.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                client.Credentials = new NetworkCredential(
                    _options.UserName, _options.Password);
            }

            await client.SendMailAsync(message);
        }
    }
}
