using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EmployeeManagementSystem.BusinessLogic.Helpers
{
    public class EmailSender(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task SendAsync(string to, string subject, string body, string htmlBody)
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("test.dotnet@etatvasoft.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            var smtpHost = _configuration["Smtp:SmtpHost"] ?? throw new ArgumentNullException("Smtp:SmtpHost");
            var smtpPort = _configuration["Smtp:SmtpPort"] ?? throw new ArgumentNullException("Smtp:SmtpPort");
            await smtp.ConnectAsync(smtpHost, int.Parse(smtpPort), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["Smtp:SmtpUser"], _configuration["Smtp:SmtpPass"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

