using DeskReservationApp.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using DeskReservationApp.Infrastructure.Persistance.Configurations;

namespace DeskReservationApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Send real email using SmtpClient
                using var mail = new MailMessage();
                mail.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using var smtp = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
                smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(mail);
                
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}. Error: {ErrorMessage}", toEmail, ex.Message);
            }
        }
    }
}
